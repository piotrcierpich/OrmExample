CREATE TABLE [dbo].[DiscountPolicies] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [DayDate]            DATETIME       NULL,
    [FromTo_Start]       DATETIME       NULL,
    [FromTo_End]         DATETIME       NULL,
    [Discriminator]      NVARCHAR (MAX) NULL,
    [DiscountPercentage] INT NULL,
    CONSTRAINT [PK_dbo.DiscountPolicies] PRIMARY KEY CLUSTERED ([Id] ASC)
);