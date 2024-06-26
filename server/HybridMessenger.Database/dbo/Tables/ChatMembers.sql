﻿CREATE TABLE [dbo].[ChatMembers] (
    [ChatId]   INT           NOT NULL,
    [UserId]   INT           NOT NULL,
    [JoinedAt] DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_ChatMembers] PRIMARY KEY CLUSTERED ([ChatId] ASC, [UserId] ASC),
    CONSTRAINT [FK_ChatMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ChatMembers_Chats_ChatId] FOREIGN KEY ([ChatId]) REFERENCES [dbo].[Chats] ([ChatId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ChatMembers_UserId]
    ON [dbo].[ChatMembers]([UserId] ASC);

