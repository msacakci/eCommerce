-- SQLite

CREATE TABLE product_Categories(
    id INT AUTO_INCREMENT PRIMARY KEY,
    product_type VARCHAR(100)
);

CREATE TABLE products(
    id INT AUTO_INCREMENT PRIMARY KEY,
    product_name VARCHAR(100),
    product_type_id INT,
    FOREIGN KEY( product_type_id) REFERENCES product_Categories(id)
);

CREATE TABLE buyers(
    id int AUTO_INCREMENT PRIMARY KEY,
    userName VARCHAR(100) NOT NULL,
    passwordHash BLOB NOT NULL,
    passwordSalt BLOB NOT NULL,
    product_id INT,
    FOREIGN KEY(product_id) REFERENCES products(id)
);

CREATE TABLE sellers(
    id int AUTO_INCREMENT PRIMARY KEY,
    userName VARCHAR(100) NOT NULL,
    passwordHash BLOB NOT NULL,
    passwordSalt BLOB NOT NULL,
    product_id INT,
    FOREIGN KEY(product_id) REFERENCES products(id)
);