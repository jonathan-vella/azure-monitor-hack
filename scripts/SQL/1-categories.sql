create table Category
(
    CategoryId int identity
        constraint Category_pk
            primary key,
    Name       nvarchar(max) not null
)