CREATE TABLE [dbo].[Messages] (
    [MessageID]   INT              IDENTITY (1, 1) NOT NULL,
    [ChatID]      UNIQUEIDENTIFIER NOT NULL,
    [UserID]      UNIQUEIDENTIFIER NOT NULL,
    [MessageText] NVARCHAR (MAX)   NOT NULL,
    [SentAt]      DATETIME2 (7)    NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([MessageID] ASC),
    CONSTRAINT [FK_Messages_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Messages_Chats_ChatID] FOREIGN KEY ([ChatID]) REFERENCES [dbo].[Chats] ([ChatID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_ChatID]
    ON [dbo].[Messages]([ChatID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_UserID]
    ON [dbo].[Messages]([UserID] ASC);

