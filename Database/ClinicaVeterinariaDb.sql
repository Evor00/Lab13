IF DB_ID('ClinicaVeterinariaDb') IS NULL CREATE DATABASE ClinicaVeterinariaDb;
GO
USE ClinicaVeterinariaDb;
GO
CREATE TABLE Owners (
    OwnerId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(120) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE
);
CREATE TABLE Pets (
    PetId INT IDENTITY(1,1) PRIMARY KEY,
    OwnerId INT NOT NULL,
    Name NVARCHAR(60) NOT NULL,
    Species NVARCHAR(30) NOT NULL,
    Breed NVARCHAR(60) NOT NULL,
    BirthDate DATE NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Pets_Owners FOREIGN KEY (OwnerId) REFERENCES Owners(OwnerId),
    CONSTRAINT UQ_Pets_Owner_Name UNIQUE (OwnerId, Name)
);
INSERT INTO Owners (FullName, Phone, Email) VALUES
('Lucia Torres', '999123456', 'lucia.torres@example.com'),
('Diego Ramos', '988654321', 'diego.ramos@example.com');
INSERT INTO Pets (OwnerId, Name, Species, Breed, BirthDate, IsActive) VALUES
(1, 'Milo', 'Perro', 'Labrador', '2021-05-10', 1),
(1, 'Nina', 'Gato', 'Siamés', '2022-08-22', 1),
(2, 'Rocky', 'Perro', 'Beagle', '2020-02-15', 0);
