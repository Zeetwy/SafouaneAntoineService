USE [DBSafouaneAntoine]
GO

/****** Object:  Table [dbo].[ServiceRendered]    Script Date: 14/05/2024 18:18:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServiceRendered](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [Status] [int] NOT NULL,
    [Date] [datetime] NULL, 
    [NumberOfHours] [int] NULL,
    [serviceOffer_id] [int] NOT NULL,
    [customer_id] [int] NOT NULL,
    CONSTRAINT [PK_ServiceRendered] PRIMARY KEY CLUSTERED (
	    [id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
    CONSTRAINT [FK_ServiceRendered_ServiceOffer] FOREIGN KEY ([serviceOffer_id]) REFERENCES [dbo].[ServiceOffer] ([id]),
    CONSTRAINT [FK_ServiceRendered_User_Customer] FOREIGN KEY ([customer_id]) REFERENCES [dbo].[User] ([id])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO