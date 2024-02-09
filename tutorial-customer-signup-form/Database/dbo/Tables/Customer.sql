CREATE TABLE [dbo].[Customer]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Salutation] NCHAR(10) NOT NULL, 
    [Title] NCHAR(10) NULL, 
    [Surname] NVARCHAR(50) NOT NULL, 
    [Forname] NVARCHAR(50) NOT NULL, 
    [Street] NVARCHAR(100) NOT NULL, 
    [Zip] NVARCHAR(50) NOT NULL, 
    [Region] NVARCHAR(50) NOT NULL, 
    [Country] NVARCHAR(50) NOT NULL, 
    [Mail] NVARCHAR(50) NOT NULL, 
    [Phone] NVARCHAR(50) NULL, 
    [Smartphone] NVARCHAR(100) NULL, 
    [Birthday] DATE NOT NULL
)
  