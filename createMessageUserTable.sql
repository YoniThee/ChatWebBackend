USE [myChatApp]
GO

/****** Object:  Table [dbo].[MessageUser]    Script Date: 06/11/2024 11:54:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MessageUser](
	[UserName] [nvarchar](450) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[ChatTeamChatRoom] [nvarchar](450) NULL,
	[messageId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_MessageUser] PRIMARY KEY CLUSTERED 
(
	[messageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[MessageUser]  WITH CHECK ADD  CONSTRAINT [FK_MessageUser_Teams_ChatTeamChatRoom] FOREIGN KEY([ChatTeamChatRoom])
REFERENCES [dbo].[Teams] ([ChatRoom])
GO

ALTER TABLE [dbo].[MessageUser] CHECK CONSTRAINT [FK_MessageUser_Teams_ChatTeamChatRoom]
GO

