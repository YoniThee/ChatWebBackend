USE [myChatApp]
GO

/****** Object:  Table [dbo].[Connections]    Script Date: 06/11/2024 11:51:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Connections](
	[UserName] [nvarchar](450) NOT NULL,
	[ChatRoom] [nvarchar](450) NOT NULL,
	[UpdatedLastMessage] [bit] NOT NULL,
	[ConnectionId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_Connections] PRIMARY KEY CLUSTERED 
(
	[ConnectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Connections]  WITH CHECK ADD  CONSTRAINT [FK_Connections_Teams_ChatTeamChatRoom] FOREIGN KEY([ChatRoom])
REFERENCES [dbo].[Teams] ([ChatRoom])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Connections] CHECK CONSTRAINT [FK_Connections_Teams_ChatTeamChatRoom]
GO

