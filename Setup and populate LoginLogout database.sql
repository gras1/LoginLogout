CREATE DATABASE LoginLogout;

USE LoginLogout;

CREATE TABLE RegisteredUsers (
    Id int AUTO_INCREMENT primary key NOT NULL,
    EmailAddress varchar(150) NOT NULL,
    HashedPassword varchar(1000) NOT NULL,
    CreatedDateTime datetime NOT NULL,
    `Status` tinyint NOT NULL,
    LockedOutUntilDateTime datetime,
    FailedLoginAttempts tinyint NOT NULL,
    PublicId BINARY(16) UNIQUE NOT NULL
);

CREATE TABLE RegisteredUserAudits (
    Id int AUTO_INCREMENT primary key NOT NULL,
    `TimeStamp` datetime NOT NULL,
    RegisteredUserId int NOT NULL,
    Activity varchar(100) NOT NULL,
    StatusBefore tinyint NOT NULL,
    StatusAfter tinyint NOT NULL
);

INSERT INTO RegisteredUsers (
    EmailAddress,
    HashedPassword,
    CreatedDateTime,
    `Status`,
    LockedOutUntilDateTime,
    FailedLoginAttempts,
    PublicId
)
VALUES (
    'test@test.com',
    '$2a$12$Sk5hIdhfv9KKHbyWMUQYAuVaUgYxXVjf8n8PsW6scntb8xvMBseta',
    '2021-09-27 00:00:01',
    2,
    NULL,
    0,
    UUID_TO_BIN(UUID())
);

INSERT INTO RegisteredUserAudits (
    `TimeStamp`,
    RegisteredUserId,
    Activity,
    StatusBefore,
    StatusAfter
)
VALUES (
    '2021-09-27 00:00:01',
    1,
    'Registered User Created',
    0,
    1
);

INSERT INTO RegisteredUserAudits (
    `TimeStamp`,
    RegisteredUserId,
    Activity,
    StatusBefore,
    StatusAfter
)
VALUES (
    '2021-09-27 00:01:01',
    1,
    'Registered User Verified',
    1,
    2
);

CREATE USER 'loginlogoutuser'@'localhost' IDENTIFIED WITH mysql_native_password BY 'l0g1nl0g0ut';

GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, INDEX, DROP, ALTER, CREATE TEMPORARY TABLES, LOCK TABLES ON LoginLogout.* TO 'loginlogoutuser'@'localhost';