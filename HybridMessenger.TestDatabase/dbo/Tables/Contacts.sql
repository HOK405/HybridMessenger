CREATE TABLE [dbo].[Contacts] (
    [UserId]        INT           NOT NULL,
    [ContactUserId] INT           NOT NULL,
    [AddedAt]       DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED ([UserId] ASC, [ContactUserId] ASC),
    CONSTRAINT [FK_Contacts_AspNetUsers_ContactUserId] FOREIGN KEY ([ContactUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_Contacts_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Contacts_ContactUserId]
    ON [dbo].[Contacts]([ContactUserId] ASC);

