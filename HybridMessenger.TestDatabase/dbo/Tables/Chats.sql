CREATE TABLE [dbo].[Chats] (
    [ChatId]    INT            IDENTITY (1, 1) NOT NULL,
    [ChatName]  NVARCHAR (255) NULL,
    [IsGroup]   BIT            NOT NULL,
    [CreatedAt] DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Chats] PRIMARY KEY CLUSTERED ([ChatId] ASC)
);

