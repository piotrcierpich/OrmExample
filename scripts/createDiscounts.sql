CREATE TABLE [dbo].[Discounts] (
    [Id]         INT IDENTITY (1, 1) NOT NULL,
    [Product_Id] INT NULL
    CONSTRAINT [PK_dbo.Discounts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Discounts_dbo.Products_Product_Id] FOREIGN KEY ([Product_Id]) REFERENCES [dbo].[Products] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Product_Id]
    ON [dbo].[Discounts]([Product_Id] ASC);