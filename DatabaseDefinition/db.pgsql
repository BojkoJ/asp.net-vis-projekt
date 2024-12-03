-- Tabulka pro kategorie produktů
CREATE TABLE Categories (
    CategoryId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Name VARCHAR(100) NOT NULL
);

-- Tabulka pro produkty
CREATE TABLE Products (
    ProductId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Price DECIMAL(10, 2) NOT NULL,
    CategoryId INT NOT NULL,
    ImgUrl VARCHAR(255),
    CONSTRAINT fk_category
        FOREIGN KEY(CategoryId)
        REFERENCES Categories(CategoryId)
);

-- Tabulka pro varianty produktů (např. různé velikosti, barvy)
CREATE TABLE ProductVariants (
    ProductVariantId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Size VARCHAR(50),
    Color VARCHAR(50),
    StockQuantity INT DEFAULT 0,
    ProductId INT NOT NULL,
    CONSTRAINT fk_product
        FOREIGN KEY(ProductId)
        REFERENCES Products(ProductId)
);

CREATE TABLE Users (
    UserId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL, -- Heslo pro uživatele
    Role VARCHAR(50) NOT NULL -- Admin, Employee, Customer
);

-- Tabulka pro objednávky
CREATE TABLE Orders (
    OrderId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    OrderDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(50) NOT NULL,
    UserId INT NOT NULL,
    CONSTRAINT fk_user
        FOREIGN KEY(UserId)
        REFERENCES Users(UserId)
);

-- Tabulka pro položky objednávky
CREATE TABLE OrderItems (
    OrderItemId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Quantity INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    ProductVariantId INT NOT NULL,
    OrderId INT NOT NULL,
    CONSTRAINT fk_order
        FOREIGN KEY(OrderId)
        REFERENCES Orders(OrderId),
    CONSTRAINT fk_product_variant
        FOREIGN KEY(ProductVariantId)
        REFERENCES ProductVariants(ProductVariantId)
);

-- Tabulka pro sklad a doskladnění produktů
CREATE TABLE Stock (
    StockId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    ProductVariantId INT NOT NULL,
    Quantity INT NOT NULL,
    RestockedDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_product_variant_stock
        FOREIGN KEY(ProductVariantId)
        REFERENCES ProductVariants(ProductVariantId)
);

-- Tabulka pro logování aktivit uživatelů v administračním systému
CREATE TABLE ActivityLogs (
    ActivityLogId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Action VARCHAR(255) NOT NULL,
    ActionDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UserId INT NOT NULL,
    Details TEXT,
    CONSTRAINT fk_user_log
        FOREIGN KEY(UserId)
        REFERENCES Users(UserId)
);