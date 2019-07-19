if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'Log')
    create table dbo.[Log] (
        [Id] int identity(1, 1) not null,
        [Date] datetimeoffset not null,
        [Level] nvarchar(50) not null,
        [Logger] nvarchar(255) not null,
        [Thread] nvarchar(255) not null,
        [Username] nvarchar(255) not null,
        [Message] nvarchar(max) not null,
        [Exception] nvarchar(max) null
    );
