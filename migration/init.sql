/*	аккаунт системы */
CREATE TABLE account (
	id INT IDENTITY(1,1) PRIMARY KEY,
	is_removed BIT NOT NULL DEFAULT 0,
	type VARCHAR(63) NOT NULL,
	created_at datetime NOT NULL,
	login VARCHAR(63) NOT NULL,
	password VARCHAR(63) NOT NULL,
	full_name VARCHAR(63) NOT NULL
);

/*	уникальный логин для не удаленного аккаунта */
CREATE UNIQUE NONCLUSTERED INDEX unique_login_account
ON account(login) WHERE is_removed = 0;
go
/*	заказ клиента */
CREATE TABLE client_order (
	id INT IDENTITY(1,1) PRIMARY KEY,
	created_at datetime NOT NULL,
	status VARCHAR(63) NOT NULL,
	price NUMERIC(12,2),
	name VARCHAR(63) NOT NULL,
	client_id INT FOREIGN KEY REFERENCES account(id) NOT NULL,
	CHECK(price >= 0)
);

/*	один заказ в статусе CREATED на одного клиента */
CREATE UNIQUE NONCLUSTERED INDEX unique_created_order
ON client_order(client_id, status ) WHERE status = 'NEW';

/*	производитель */
CREATE TABLE producer (
	id INT IDENTITY(1,1) PRIMARY KEY,
	name VARCHAR(63) UNIQUE NOT NULL,
	description VARCHAR(255)
);
/*	компонент */
CREATE TABLE computer_component (
	id INT IDENTITY(1,1) PRIMARY KEY,
	type VARCHAR(63) NOT NULL,
	name VARCHAR(63) NOT NULL,
	description TEXT,
	trade_price NUMERIC(12,2),
	producer_id INT FOREIGN KEY REFERENCES producer(id) NOT NULL,
	CHECK(trade_price > 0)
);
/*	сборка заказа */
CREATE TABLE client_assembly_details (
	id INT IDENTITY(1,1) PRIMARY KEY,
	count INT NOT NULL,
	client_order_id INT FOREIGN KEY REFERENCES client_order(id) NOT NULL,
	computer_component_id INT FOREIGN KEY REFERENCES computer_component(id) NOT NULL,
	CHECK(count >= 0)
);
/*	склад магазина */
CREATE TABLE shop_store (
	id INT IDENTITY(1,1) PRIMARY KEY,
	count INT DEFAULT 0 NOT NULL,
	computer_component_id INT FOREIGN KEY REFERENCES computer_component(id) NOT NULL,
	CHECK(count >= 0)
);
/*	история склада магазина */
CREATE TABLE shop_store_history (
	id INT IDENTITY(1,1) PRIMARY KEY,
	count INT DEFAULT 0 NOT NULL,
	type VARCHAR(63) NOT NULL,
	message VARCHAR(255) NOT NULL,
	shop_store_id INT FOREIGN KEY REFERENCES shop_store(id) NOT NULL,
	CHECK(count >= 0)
);
/*	производитель */
CREATE TABLE supplier (
	id INT IDENTITY(1,1) PRIMARY KEY,
	is_removed BIT NOT NULL DEFAULT 0,
	name VARCHAR(63) UNIQUE NOT NULL,
	description VARCHAR(255),
	account_id INT FOREIGN KEY REFERENCES account(id) NOT NULL
);
/*	склад поставщика */
CREATE TABLE supplier_store (
	id INT IDENTITY(1,1) PRIMARY KEY,
	count INT DEFAULT 0 NOT NULL,
	supplier_price NUMERIC(12,2) NOT NULL,
	computer_component_id INT FOREIGN KEY REFERENCES computer_component(id) NOT NULL,
	supplier_id INT FOREIGN KEY REFERENCES supplier(id) NOT NULL,
	UNIQUE (computer_component_id, supplier_id),
	CHECK(supplier_price > 0)
);
/*	заказ поставщику */
CREATE TABLE supply_assembly_order (
	id INT IDENTITY(1,1) PRIMARY KEY,
	created_at datetime NOT NULL,
	delivered_at datetime,
	name VARCHAR(63) NOT NULL,
	price NUMERIC(12,2),
	status VARCHAR(63) NOT NULL,
	supplier_id INT FOREIGN KEY REFERENCES supplier(id) NOT NULL
);

/*	один заказ в статусе NEW на одного поставщика */
CREATE UNIQUE NONCLUSTERED INDEX unique_new_supply_assembly_order
ON supply_assembly_order(supplier_id, status ) WHERE status = 'NEW';

/*	сборка заказа для поставщика */
CREATE TABLE supply_assembly_details (
	id INT IDENTITY(1,1) PRIMARY KEY,
	count INT NOT NULL,
	supply_assembly_order_id INT FOREIGN KEY REFERENCES supply_assembly_order(id) NOT NULL,
	supplier_store_id INT FOREIGN KEY REFERENCES supplier_store(id) NOT NULL,
	CHECK(count>=0)
);

INSERT INTO producer(name) VALUES('INTEL'),('AMD'),('ATMEGA'),('XEON');
go
CREATE TRIGGER insert_new_detail_in_client_order
ON client_assembly_details
FOR INSERT
AS 
DECLARE 
	@component_trade_price NUMERIC(12,2),
	@computer_component_id INT,
	@client_order_id INT,
	@component_count INT
BEGIN
	SELECT @client_order_id = (SELECT client_order_id FROM inserted)
	SELECT @computer_component_id = (SELECT computer_component_id FROM inserted)
	SELECT @component_count = (SELECT count FROM inserted)
	SELECT @component_trade_price = (SELECT trade_price FROM computer_component WHERE id = @computer_component_id)
	UPDATE client_order SET price = COALESCE(price, 0) + @component_trade_price * @component_count WHERE id = @client_order_id;
END
go
CREATE TRIGGER update_new_detail_in_client_order
ON client_assembly_details
FOR UPDATE
AS 
DECLARE 
	@component_trade_price NUMERIC(12,2),
	@computer_component_id INT,
	@client_order_id INT,
	@component_count INT,
	@deleted_component_count INT
BEGIN
	SELECT @client_order_id = (SELECT client_order_id FROM inserted)
	SELECT @computer_component_id = (SELECT computer_component_id FROM inserted)
	SELECT @component_count = (SELECT count FROM inserted)
	SELECT @deleted_component_count = (SELECT count FROM deleted)

	SELECT @component_trade_price = (SELECT trade_price FROM computer_component WHERE id = @computer_component_id)
	UPDATE client_order SET price = COALESCE(price, 0) + (@component_count - @deleted_component_count) * @component_trade_price 
	WHERE id = @client_order_id;
END
go
CREATE TRIGGER delete_detail_from_client_order
ON client_assembly_details
FOR DELETE
AS 
DECLARE 
	@component_trade_price NUMERIC(12,2),
	@computer_component_id INT,
	@client_order_id INT,
	@component_count INT
BEGIN
	declare client_assembly_details_cursor cursor for
	select client_order_id, computer_component_id, count from deleted;

	open client_assembly_details_cursor;
	fetch next from client_assembly_details_cursor into @client_order_id, @computer_component_id, @component_count;
	while @@FETCH_STATUS = 0 
		begin
			SELECT @component_trade_price = (SELECT trade_price FROM computer_component WHERE id = @computer_component_id);
			UPDATE client_order SET price = price - @component_trade_price * @component_count WHERE id = @client_order_id;
			fetch next from client_assembly_details_cursor into @client_order_id, @computer_component_id, @component_count;
		end
	close client_assembly_details_cursor;
	deallocate client_assembly_details_cursor;
END
go
--triggers for supplier
CREATE TRIGGER insert_new_supplier_detail_in_order
ON supply_assembly_details
FOR INSERT
AS 
DECLARE 
	@price NUMERIC(12,2),
	@supplier_store_id INT,
	@supply_assembly_order_id INT,
	@component_count INT
BEGIN
	SELECT @supplier_store_id = (SELECT supplier_store_id FROM inserted)
	SELECT @supply_assembly_order_id = (SELECT supply_assembly_order_id FROM inserted)
	SELECT @component_count = (SELECT count FROM inserted)
	SELECT @price = (SELECT supplier_price FROM supplier_store WHERE id = @supplier_store_id)
	UPDATE supply_assembly_order SET price = COALESCE(price, 0) + @price * @component_count WHERE id = @supply_assembly_order_id;
END
go
CREATE TRIGGER update_new_supplier_detail_in_order
ON supply_assembly_details
FOR UPDATE
AS 
DECLARE 
	@store_price NUMERIC(12,2),
	@supplier_store_id INT,
	@supply_assembly_order_id INT,
	@component_count INT,
	@deleted_component_count INT
BEGIN
	SELECT @supplier_store_id = (SELECT supplier_store_id FROM inserted)
	SELECT @supply_assembly_order_id = (SELECT supply_assembly_order_id FROM inserted)
	SELECT @component_count = (SELECT count FROM inserted)
	SELECT @deleted_component_count = (SELECT count FROM deleted)
	SELECT @store_price = (SELECT supplier_price FROM supplier_store WHERE id = @supplier_store_id)
	UPDATE supply_assembly_order SET price = COALESCE(price, 0) + @store_price * (@component_count - @deleted_component_count) 
	WHERE id = @supply_assembly_order_id;
END
go
CREATE TRIGGER delete_supplier_detail_from_order
ON supply_assembly_details
FOR DELETE
AS 
DECLARE 
	@price NUMERIC(12,2),
	@supplier_store_id INT,
	@supply_assembly_order_id INT,
	@component_count INT
BEGIN
	declare supplier_assembly_details_cursor cursor for
	select supply_assembly_order_id, supplier_store_id, count from deleted;

	open supplier_assembly_details_cursor;
	fetch next from supplier_assembly_details_cursor into @supply_assembly_order_id, @supplier_store_id, @component_count;
	while @@FETCH_STATUS = 0 
		begin
			SELECT @price = (SELECT supplier_price FROM supplier_store WHERE id = @supplier_store_id);
			UPDATE supply_assembly_order SET price = price - @price * @component_count WHERE id = @supply_assembly_order_id;
			fetch next from supplier_assembly_details_cursor into @supply_assembly_order_id, @supplier_store_id, @component_count;
		end
	close supplier_assembly_details_cursor;
	deallocate supplier_assembly_details_cursor;
END