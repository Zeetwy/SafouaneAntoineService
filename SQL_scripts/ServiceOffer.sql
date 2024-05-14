USE [DBSafouaneAntoine]
GO

/****** Object:  Table [dbo].[ServiceOffer]    Script Date: 10/05/2024 07:03:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServiceOffer](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type] [varchar](50) NOT NULL,
	[description] [text] NULL,
	[category_id] [int] NOT NULL,
	[user_id] [int] NOT NULL,
    CONSTRAINT [PK_ServiceOffer] PRIMARY KEY CLUSTERED (
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
    CONSTRAINT [FK_ServiceOffer_User] FOREIGN KEY ([user_id]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_ServiceOffer_ServiceCategory] FOREIGN KEY ([category_id]) REFERENCES [dbo].[ServiceCategory] ([id])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

