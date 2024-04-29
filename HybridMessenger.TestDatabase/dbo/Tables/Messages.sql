CREATE TABLE [dbo].[Messages] (
    [MessageId]   INT            IDENTITY (1, 1) NOT NULL,
    [ChatId]      INT            NOT NULL,
    [UserId]      INT            NOT NULL,
    [MessageText] NVARCHAR (MAX) NOT NULL,
    [SentAt]      DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([MessageId] ASC),
    CONSTRAINT [FK_Messages_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Messages_Chats_ChatId] FOREIGN KEY ([ChatId]) REFERENCES [dbo].[Chats] ([ChatId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_ChatId]
    ON [dbo].[Messages]([ChatId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_UserId]
    ON [dbo].[Messages]([UserId] ASC);

