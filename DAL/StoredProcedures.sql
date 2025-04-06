CREATE PROCEDURE CreateBike
    @Type NVARCHAR(50),
    @Status NVARCHAR(50),
    @RentalPricePerHour DECIMAL(18,2),
    @Model NVARCHAR(50),
    @Image VARBINARY(MAX),
    @HasInsurance BIT
AS
BEGIN
    INSERT INTO Bike (Type, Status, RentalPricePerHour, Model, Image, HasInsurance)
    VALUES (@Type, @Status, @RentalPricePerHour, @Model, @Image, @HasInsurance);
END
GO

CREATE PROCEDURE GetAllBikes
AS
BEGIN
    SELECT BikeID, Type, Status, DateAdded, RentalPricePerHour, Model, Image, HasInsurance
    FROM Bike;
END
GO

CREATE PROCEDURE GetBikeById
    @BikeID INT
AS
BEGIN
    SELECT BikeID, Type, Status, DateAdded, Image, RentalPricePerHour, Model, HasInsurance
    FROM Bike
    WHERE BikeID = @BikeID;
END
GO

CREATE PROCEDURE UpdateBike
    @BikeID INT,
    @Type NVARCHAR(50),
    @Status NVARCHAR(50),
    @RentalPricePerHour DECIMAL(18,2),
    @Model NVARCHAR(50),
    @Image VARBINARY(MAX),
    @HasInsurance BIT
AS
BEGIN
    UPDATE Bike
    SET Type = @Type,
        Status = @Status,
        RentalPricePerHour = @RentalPricePerHour,
        Model = @Model,
        Image = @Image,
        HasInsurance = @HasInsurance
    WHERE BikeID = @BikeID;
END
GO

CREATE PROCEDURE DeleteBike
    @BikeID INT
AS
BEGIN
    DELETE FROM Bike
    WHERE BikeID = @BikeID;
END
GO

-- Stored procedure for XML export using your actual schema
CREATE PROCEDURE [dbo].[ExportRentalsToXML]
    @CustomerName NVARCHAR(100) = NULL,
    @BikeModel NVARCHAR(50) = NULL,
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        (SELECT 
            r.RentalID AS '@ID',
            r.RentalStartDate AS 'RentalStartDate',
            r.RentalEndDate AS 'RentalEndDate',
            r.RentalDuration AS 'RentalDuration',
            r.TotalCost AS 'TotalCost',
            (SELECT 
                b.BikeID AS '@ID',
                b.Model AS 'Model',
                b.Type AS 'Type',
                b.Status AS 'Status',
                b.RentalPricePerHour AS 'RentalPricePerHour',
                b.DateAdded AS 'DateAdded',
                b.HasInsurance AS 'HasInsurance'
            FOR XML PATH('Bike'), TYPE),
            (SELECT 
                c.CustomerID AS '@ID',
                c.FullName AS 'FullName',
                c.Email AS 'Email',
                c.PhoneNumber AS 'PhoneNumber',
                c.Address AS 'Address',
                c.DateOfBirth AS 'DateOfBirth',
                c.IsMarried AS 'IsMarried'
            FOR XML PATH('Customer'), TYPE)
        FROM Rental r
        LEFT JOIN Bike b ON r.BikeID = b.BikeID
        LEFT JOIN Customer c ON r.CustomerID = c.CustomerID
        WHERE 
            (@CustomerName IS NULL OR c.FullName LIKE '%' + @CustomerName + '%') AND
            (@BikeModel IS NULL OR b.Model LIKE '%' + @BikeModel + '%') AND
            (@StartDate IS NULL OR r.RentalStartDate >= @StartDate) AND
            (@EndDate IS NULL OR r.RentalEndDate <= @EndDate)
        FOR XML PATH('Rental'), ROOT('Rentals'), ELEMENTS)
END
GO

-- Stored procedure for JSON export using your actual schema
CREATE PROCEDURE [dbo].[ExportRentalsToJSON]
    @CustomerName NVARCHAR(100) = NULL,
    @BikeModel NVARCHAR(50) = NULL,
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @json NVARCHAR(MAX);

    SELECT @json = (
        SELECT 
            r.RentalID,
            r.RentalStartDate,
            r.RentalEndDate,
            r.RentalDuration,
            r.TotalCost,
            JSON_QUERY((SELECT 
                b.BikeID,
                b.Model,
                b.Type,
                b.Status,
                b.RentalPricePerHour,
                b.DateAdded,
                b.HasInsurance
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)) AS Bike,
            JSON_QUERY((SELECT 
                c.CustomerID,
                c.FullName,
                c.Email,
                c.PhoneNumber,
                c.Address,
                c.DateOfBirth,
                FORMAT(c.DateOfBirth, 'yyyy-MM-dd') AS DateOfBirthFormatted,
                c.IsMarried
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)) AS Customer
        FROM Rental r
        LEFT JOIN Bike b ON r.BikeID = b.BikeID
        LEFT JOIN Customer c ON r.CustomerID = c.CustomerID
        WHERE 
            (@CustomerName IS NULL OR c.FullName LIKE '%' + @CustomerName + '%') AND
            (@BikeModel IS NULL OR b.Model LIKE '%' + @BikeModel + '%') AND
            (@StartDate IS NULL OR r.RentalStartDate >= @StartDate) AND
            (@EndDate IS NULL OR r.RentalEndDate <= @EndDate)
        FOR JSON PATH, ROOT('Rentals')
    );

    SELECT @json;
END
GO