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
