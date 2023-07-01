CREATE DATABASE Registro
GO
USE Registro
GO
CREATE TABLE Usuario(
UsuarioID INT IDENTITY PRIMARY KEY,
Username VARCHAR(25) NOT NULL,
Contrase�a VARCHAR(300) NOT NULL,
Email VARCHAR(150) NOT NULL,
FechaRegistro DATETIME NOT NULL,
Estado TINYINT NOT NULL,
Tipo TINYINT NOT NULL
)
GO
CREATE TABLE UsuarioSecurity(
UsuarioSecurity INT IDENTITY PRIMARY KEY,
UsuarioID INT NOT NULL,
Username VARCHAR(25) NOT NULL,
UltimoAcceso DATETIME NOT NULL,
IPAcceso VARCHAR(15)
);