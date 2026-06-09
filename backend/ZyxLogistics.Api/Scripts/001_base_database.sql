IF DB_ID(N'ZyxLogisticsDb') IS NULL
BEGIN
    CREATE DATABASE ZyxLogisticsDb;
END
GO

USE ZyxLogisticsDb;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF TYPE_ID(N'dbo.IntIdList') IS NULL
BEGIN
    EXEC(N'CREATE TYPE dbo.IntIdList AS TABLE (Id INT NOT NULL PRIMARY KEY);');
END
GO

IF OBJECT_ID(N'dbo.Perfil', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Perfil
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Perfil PRIMARY KEY,
        Descricao NVARCHAR(100) NOT NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_Perfil_Ativo DEFAULT (1),
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_Perfil_CriadoEm DEFAULT (SYSDATETIME())
    );

    CREATE UNIQUE INDEX UX_Perfil_Descricao ON dbo.Perfil(Descricao);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Perfil_Descricao' AND object_id = OBJECT_ID(N'dbo.Perfil'))
BEGIN
    CREATE UNIQUE INDEX UX_Perfil_Descricao ON dbo.Perfil(Descricao);
END
GO

IF OBJECT_ID(N'dbo.Usuario', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuario
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuario PRIMARY KEY,
        Nome NVARCHAR(150) NOT NULL,
        Email NVARCHAR(150) NOT NULL,
        Senha NVARCHAR(255) NULL,
        PerfilId INT NOT NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_Usuario_Ativo DEFAULT (1),
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_Usuario_CriadoEm DEFAULT (SYSDATETIME()),
        AtualizadoEm DATETIME2(0) NULL,
        CONSTRAINT FK_Usuario_Perfil FOREIGN KEY (PerfilId) REFERENCES dbo.Perfil(Id)
    );

    CREATE UNIQUE INDEX UX_Usuario_Email
        ON dbo.Usuario(Email)
        WHERE Ativo = 1;
END
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Usuario')
      AND name = N'Senha'
      AND is_nullable = 0
)
BEGIN
    ALTER TABLE dbo.Usuario ALTER COLUMN Senha NVARCHAR(255) NULL;
END
GO

IF EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Usuario_Email'
      AND object_id = OBJECT_ID(N'dbo.Usuario')
      AND has_filter = 0
)
BEGIN
    DROP INDEX UX_Usuario_Email ON dbo.Usuario;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Usuario_Email' AND object_id = OBJECT_ID(N'dbo.Usuario'))
BEGIN
    CREATE UNIQUE INDEX UX_Usuario_Email
        ON dbo.Usuario(Email)
        WHERE Ativo = 1;
END
GO

IF OBJECT_ID(N'dbo.Permissao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Permissao
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Permissao PRIMARY KEY,
        Codigo NVARCHAR(100) NOT NULL,
        Descricao NVARCHAR(150) NOT NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_Permissao_Ativo DEFAULT (1)
    );

    CREATE UNIQUE INDEX UX_Permissao_Codigo ON dbo.Permissao(Codigo);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Permissao_Codigo' AND object_id = OBJECT_ID(N'dbo.Permissao'))
BEGIN
    CREATE UNIQUE INDEX UX_Permissao_Codigo ON dbo.Permissao(Codigo);
END
GO

IF OBJECT_ID(N'dbo.PerfilPermissao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PerfilPermissao
    (
        PerfilId INT NOT NULL,
        PermissaoId INT NOT NULL,
        CONSTRAINT PK_PerfilPermissao PRIMARY KEY (PerfilId, PermissaoId),
        CONSTRAINT FK_PerfilPermissao_Perfil FOREIGN KEY (PerfilId) REFERENCES dbo.Perfil(Id),
        CONSTRAINT FK_PerfilPermissao_Permissao FOREIGN KEY (PermissaoId) REFERENCES dbo.Permissao(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.Transportadora', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Transportadora
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Transportadora PRIMARY KEY,
        Nome NVARCHAR(150) NOT NULL,
        Cnpj VARCHAR(20) NOT NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_Transportadora_Ativo DEFAULT (1),
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_Transportadora_CriadoEm DEFAULT (SYSDATETIME()),
        AtualizadoEm DATETIME2(0) NULL
    );

    CREATE UNIQUE INDEX UX_Transportadora_Cnpj
        ON dbo.Transportadora(Cnpj)
        WHERE Ativo = 1;
END
GO

IF EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Transportadora_Cnpj'
      AND object_id = OBJECT_ID(N'dbo.Transportadora')
      AND has_filter = 0
)
BEGIN
    DROP INDEX UX_Transportadora_Cnpj ON dbo.Transportadora;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Transportadora_Cnpj' AND object_id = OBJECT_ID(N'dbo.Transportadora'))
BEGIN
    CREATE UNIQUE INDEX UX_Transportadora_Cnpj
        ON dbo.Transportadora(Cnpj)
        WHERE Ativo = 1;
END
GO

IF OBJECT_ID(N'dbo.Operacao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Operacao
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Operacao PRIMARY KEY,
        Descricao NVARCHAR(50) NOT NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_Operacao_Ativo DEFAULT (1)
    );

    CREATE UNIQUE INDEX UX_Operacao_Descricao ON dbo.Operacao(Descricao);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Operacao_Descricao' AND object_id = OBJECT_ID(N'dbo.Operacao'))
BEGIN
    CREATE UNIQUE INDEX UX_Operacao_Descricao ON dbo.Operacao(Descricao);
END
GO

IF OBJECT_ID(N'dbo.Status', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Status
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Status PRIMARY KEY,
        Descricao NVARCHAR(50) NOT NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_Status_Ativo DEFAULT (1)
    );

    CREATE UNIQUE INDEX UX_Status_Descricao ON dbo.Status(Descricao);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Status_Descricao' AND object_id = OBJECT_ID(N'dbo.Status'))
BEGIN
    CREATE UNIQUE INDEX UX_Status_Descricao ON dbo.Status(Descricao);
END
GO

IF OBJECT_ID(N'dbo.Veiculo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Veiculo
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Veiculo PRIMARY KEY,
        Placa VARCHAR(10) NOT NULL,
        TipoVeiculo NVARCHAR(80) NOT NULL,
        TransportadoraId INT NOT NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_Veiculo_Ativo DEFAULT (1),
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_Veiculo_CriadoEm DEFAULT (SYSDATETIME()),
        AtualizadoEm DATETIME2(0) NULL,
        CONSTRAINT FK_Veiculo_Transportadora FOREIGN KEY (TransportadoraId) REFERENCES dbo.Transportadora(Id)
    );

    CREATE UNIQUE INDEX UX_Veiculo_Placa
        ON dbo.Veiculo(Placa)
        WHERE Ativo = 1;
END
GO

IF EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Veiculo_Placa'
      AND object_id = OBJECT_ID(N'dbo.Veiculo')
      AND has_filter = 0
)
BEGIN
    DROP INDEX UX_Veiculo_Placa ON dbo.Veiculo;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Veiculo_Placa' AND object_id = OBJECT_ID(N'dbo.Veiculo'))
BEGIN
    CREATE UNIQUE INDEX UX_Veiculo_Placa
        ON dbo.Veiculo(Placa)
        WHERE Ativo = 1;
END
GO

IF OBJECT_ID(N'dbo.Motorista', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Motorista
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Motorista PRIMARY KEY,
        Nome NVARCHAR(150) NOT NULL,
        Cnh VARCHAR(30) NOT NULL,
        Telefone VARCHAR(20) NOT NULL CONSTRAINT DF_Motorista_Telefone DEFAULT (''),
        Ativo BIT NOT NULL CONSTRAINT DF_Motorista_Ativo DEFAULT (1),
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_Motorista_CriadoEm DEFAULT (SYSDATETIME()),
        AtualizadoEm DATETIME2(0) NULL
    );

    CREATE UNIQUE INDEX UX_Motorista_Cnh
        ON dbo.Motorista(Cnh)
        WHERE Ativo = 1;
END
GO

IF COL_LENGTH(N'dbo.Motorista', N'Telefone') IS NULL
BEGIN
    ALTER TABLE dbo.Motorista ADD Telefone VARCHAR(20) NOT NULL CONSTRAINT DF_Motorista_Telefone DEFAULT ('');
END
GO

IF EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Motorista_Cnh'
      AND object_id = OBJECT_ID(N'dbo.Motorista')
      AND has_filter = 0
)
BEGIN
    DROP INDEX UX_Motorista_Cnh ON dbo.Motorista;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Motorista_Cnh' AND object_id = OBJECT_ID(N'dbo.Motorista'))
BEGIN
    CREATE UNIQUE INDEX UX_Motorista_Cnh
        ON dbo.Motorista(Cnh)
        WHERE Ativo = 1;
END
GO

IF OBJECT_ID(N'dbo.ConfiguracaoAgendamento', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ConfiguracaoAgendamento
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ConfiguracaoAgendamento PRIMARY KEY,
        IntervaloMinutos INT NOT NULL,
        HoraInicio TIME(0) NOT NULL,
        HoraFim TIME(0) NOT NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_ConfiguracaoAgendamento_Ativo DEFAULT (1),
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_ConfiguracaoAgendamento_CriadoEm DEFAULT (SYSDATETIME()),
        AtualizadoEm DATETIME2(0) NULL,
        CONSTRAINT CK_ConfiguracaoAgendamento_Intervalo CHECK (IntervaloMinutos > 0),
        CONSTRAINT CK_ConfiguracaoAgendamento_Horario CHECK (HoraInicio < HoraFim)
    );

    CREATE UNIQUE INDEX UX_ConfiguracaoAgendamento_Ativa
        ON dbo.ConfiguracaoAgendamento(Ativo)
        WHERE Ativo = 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ConfiguracaoAgendamento_Ativa' AND object_id = OBJECT_ID(N'dbo.ConfiguracaoAgendamento'))
BEGIN
    CREATE UNIQUE INDEX UX_ConfiguracaoAgendamento_Ativa
        ON dbo.ConfiguracaoAgendamento(Ativo)
        WHERE Ativo = 1;
END
GO

IF OBJECT_ID(N'dbo.Local', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Local
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Local PRIMARY KEY,
        Descricao NVARCHAR(100) NOT NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_Local_Ativo DEFAULT (1),
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_Local_CriadoEm DEFAULT (SYSDATETIME()),
        AtualizadoEm DATETIME2(0) NULL
    );

    CREATE UNIQUE INDEX UX_Local_Descricao
        ON dbo.Local(Descricao)
        WHERE Ativo = 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Local_Descricao' AND object_id = OBJECT_ID(N'dbo.Local'))
BEGIN
    CREATE UNIQUE INDEX UX_Local_Descricao
        ON dbo.Local(Descricao)
        WHERE Ativo = 1;
END
GO

IF OBJECT_ID(N'dbo.Produto', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Produto
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Produto PRIMARY KEY,
        Descricao NVARCHAR(150) NOT NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_Produto_Ativo DEFAULT (1),
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_Produto_CriadoEm DEFAULT (SYSDATETIME()),
        AtualizadoEm DATETIME2(0) NULL
    );

    CREATE UNIQUE INDEX UX_Produto_Descricao
        ON dbo.Produto(Descricao)
        WHERE Ativo = 1;
END
GO

IF EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Produto_Descricao'
      AND object_id = OBJECT_ID(N'dbo.Produto')
      AND has_filter = 0
)
BEGIN
    DROP INDEX UX_Produto_Descricao ON dbo.Produto;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Produto_Descricao' AND object_id = OBJECT_ID(N'dbo.Produto'))
BEGIN
    CREATE UNIQUE INDEX UX_Produto_Descricao
        ON dbo.Produto(Descricao)
        WHERE Ativo = 1;
END
GO

IF OBJECT_ID(N'dbo.Agendamento', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Agendamento
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Agendamento PRIMARY KEY,
        OperacaoId INT NOT NULL,
        StatusId INT NOT NULL,
        TransportadoraId INT NOT NULL,
        LocalId INT NULL,
        VeiculoId INT NOT NULL,
        MotoristaId INT NOT NULL,
        DataHoraAgendada DATETIME2(0) NOT NULL,
        DataHoraChegada DATETIME2(0) NULL,
        DataHoraDoca DATETIME2(0) NULL,
        DataHoraFinalizado DATETIME2(0) NULL,
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_Agendamento_CriadoEm DEFAULT (SYSDATETIME()),
        AtualizadoEm DATETIME2(0) NULL,
        CONSTRAINT FK_Agendamento_Operacao FOREIGN KEY (OperacaoId) REFERENCES dbo.Operacao(Id),
        CONSTRAINT FK_Agendamento_Status FOREIGN KEY (StatusId) REFERENCES dbo.Status(Id),
        CONSTRAINT FK_Agendamento_Transportadora FOREIGN KEY (TransportadoraId) REFERENCES dbo.Transportadora(Id),
        CONSTRAINT FK_Agendamento_Local FOREIGN KEY (LocalId) REFERENCES dbo.Local(Id),
        CONSTRAINT FK_Agendamento_Veiculo FOREIGN KEY (VeiculoId) REFERENCES dbo.Veiculo(Id),
        CONSTRAINT FK_Agendamento_Motorista FOREIGN KEY (MotoristaId) REFERENCES dbo.Motorista(Id)
    );

    CREATE INDEX IX_Agendamento_DataHoraAgendada ON dbo.Agendamento(DataHoraAgendada);

    CREATE UNIQUE INDEX UX_Agendamento_Motorista_Ativo
        ON dbo.Agendamento(MotoristaId)
        WHERE StatusId IN (1, 2, 3);

    CREATE UNIQUE INDEX UX_Agendamento_Veiculo_Ativo
        ON dbo.Agendamento(VeiculoId)
        WHERE StatusId IN (1, 2, 3);
END
GO

IF COL_LENGTH(N'dbo.Agendamento', N'TransportadoraId') IS NULL
BEGIN
    ALTER TABLE dbo.Agendamento ADD TransportadoraId INT NULL;
END
GO

UPDATE a
SET TransportadoraId = v.TransportadoraId
FROM dbo.Agendamento a
INNER JOIN dbo.Veiculo v ON v.Id = a.VeiculoId
WHERE a.TransportadoraId IS NULL;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Agendamento')
      AND name = N'TransportadoraId'
      AND is_nullable = 1
)
AND NOT EXISTS (SELECT 1 FROM dbo.Agendamento WHERE TransportadoraId IS NULL)
BEGIN
    ALTER TABLE dbo.Agendamento ALTER COLUMN TransportadoraId INT NOT NULL;
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Agendamento_Transportadora'
      AND parent_object_id = OBJECT_ID(N'dbo.Agendamento')
)
BEGIN
    ALTER TABLE dbo.Agendamento
    ADD CONSTRAINT FK_Agendamento_Transportadora FOREIGN KEY (TransportadoraId) REFERENCES dbo.Transportadora(Id);
END
GO

IF COL_LENGTH(N'dbo.Agendamento', N'LocalId') IS NULL
BEGIN
    ALTER TABLE dbo.Agendamento ADD LocalId INT NULL;
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Agendamento_Local'
      AND parent_object_id = OBJECT_ID(N'dbo.Agendamento')
)
BEGIN
    ALTER TABLE dbo.Agendamento
    ADD CONSTRAINT FK_Agendamento_Local FOREIGN KEY (LocalId) REFERENCES dbo.Local(Id);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Agendamento_DataHoraAgendada' AND object_id = OBJECT_ID(N'dbo.Agendamento'))
BEGIN
    CREATE INDEX IX_Agendamento_DataHoraAgendada ON dbo.Agendamento(DataHoraAgendada);
END
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Agendamento_Horario_Ativo' AND object_id = OBJECT_ID(N'dbo.Agendamento'))
BEGIN
    DROP INDEX UX_Agendamento_Horario_Ativo ON dbo.Agendamento;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Agendamento_Motorista_Ativo' AND object_id = OBJECT_ID(N'dbo.Agendamento'))
BEGIN
    CREATE UNIQUE INDEX UX_Agendamento_Motorista_Ativo
        ON dbo.Agendamento(MotoristaId)
        WHERE StatusId IN (1, 2, 3);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Agendamento_Veiculo_Ativo' AND object_id = OBJECT_ID(N'dbo.Agendamento'))
BEGIN
    CREATE UNIQUE INDEX UX_Agendamento_Veiculo_Ativo
        ON dbo.Agendamento(VeiculoId)
        WHERE StatusId IN (1, 2, 3);
END
GO

IF OBJECT_ID(N'dbo.OperacaoItem', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperacaoItem
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OperacaoItem PRIMARY KEY,
        AgendamentoId INT NOT NULL,
        ProdutoId INT NOT NULL,
        Quantidade DECIMAL(18,3) NOT NULL,
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_OperacaoItem_CriadoEm DEFAULT (SYSDATETIME()),
        CONSTRAINT FK_OperacaoItem_Agendamento FOREIGN KEY (AgendamentoId) REFERENCES dbo.Agendamento(Id),
        CONSTRAINT FK_OperacaoItem_Produto FOREIGN KEY (ProdutoId) REFERENCES dbo.Produto(Id),
        CONSTRAINT CK_OperacaoItem_Quantidade CHECK (Quantidade > 0)
    );

    CREATE INDEX IX_OperacaoItem_AgendamentoId ON dbo.OperacaoItem(AgendamentoId);
    CREATE UNIQUE INDEX UX_OperacaoItem_Agendamento_Produto ON dbo.OperacaoItem(AgendamentoId, ProdutoId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OperacaoItem_AgendamentoId' AND object_id = OBJECT_ID(N'dbo.OperacaoItem'))
BEGIN
    CREATE INDEX IX_OperacaoItem_AgendamentoId ON dbo.OperacaoItem(AgendamentoId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_OperacaoItem_Agendamento_Produto' AND object_id = OBJECT_ID(N'dbo.OperacaoItem'))
BEGIN
    CREATE UNIQUE INDEX UX_OperacaoItem_Agendamento_Produto ON dbo.OperacaoItem(AgendamentoId, ProdutoId);
END
GO

IF COL_LENGTH(N'dbo.OperacaoItem', N'AtualizadoEm') IS NULL
BEGIN
    ALTER TABLE dbo.OperacaoItem ADD AtualizadoEm DATETIME2(0) NULL;
END
GO

IF OBJECT_ID(N'dbo.Inventario', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Inventario
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Inventario PRIMARY KEY,
        ProdutoId INT NOT NULL,
        QuantidadeAtual DECIMAL(18,3) NOT NULL CONSTRAINT DF_Inventario_QuantidadeAtual DEFAULT (0),
        DataUltimaAtualizacao DATETIME2(0) NOT NULL CONSTRAINT DF_Inventario_DataUltimaAtualizacao DEFAULT (SYSDATETIME()),
        CONSTRAINT FK_Inventario_Produto FOREIGN KEY (ProdutoId) REFERENCES dbo.Produto(Id),
        CONSTRAINT CK_Inventario_QuantidadeAtual CHECK (QuantidadeAtual >= 0)
    );

    CREATE UNIQUE INDEX UX_Inventario_ProdutoId ON dbo.Inventario(ProdutoId);
END
GO

IF OBJECT_ID(N'dbo.CheckInCodigo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CheckInCodigo
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CheckInCodigo PRIMARY KEY,
        AgendamentoId INT NOT NULL,
        MotoristaId INT NOT NULL,
        Cnh VARCHAR(30) NOT NULL,
        Telefone VARCHAR(20) NOT NULL,
        Codigo VARCHAR(6) NOT NULL,
        ExpiraEm DATETIME2(0) NOT NULL,
        UsadoEm DATETIME2(0) NULL,
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_CheckInCodigo_CriadoEm DEFAULT (SYSDATETIME()),
        CONSTRAINT FK_CheckInCodigo_Agendamento FOREIGN KEY (AgendamentoId) REFERENCES dbo.Agendamento(Id),
        CONSTRAINT FK_CheckInCodigo_Motorista FOREIGN KEY (MotoristaId) REFERENCES dbo.Motorista(Id)
    );

    CREATE INDEX IX_CheckInCodigo_Cnh_Codigo ON dbo.CheckInCodigo(Cnh, Codigo, ExpiraEm);
END
GO

IF OBJECT_ID(N'dbo.SmsEnvio', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SmsEnvio
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SmsEnvio PRIMARY KEY,
        Telefone VARCHAR(20) NOT NULL,
        Mensagem NVARCHAR(300) NOT NULL,
        CriadoEm DATETIME2(0) NOT NULL CONSTRAINT DF_SmsEnvio_CriadoEm DEFAULT (SYSDATETIME())
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Inventario_ProdutoId' AND object_id = OBJECT_ID(N'dbo.Inventario'))
BEGIN
    CREATE UNIQUE INDEX UX_Inventario_ProdutoId ON dbo.Inventario(ProdutoId);
END
GO

INSERT INTO dbo.Inventario (ProdutoId, QuantidadeAtual)
SELECT p.Id, 0
FROM dbo.Produto p
WHERE p.Ativo = 1
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.Inventario i
      WHERE i.ProdutoId = p.Id
  );
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Perfil WHERE Id IN (1, 2, 3))
BEGIN
    SET IDENTITY_INSERT dbo.Perfil ON;

    INSERT INTO dbo.Perfil (Id, Descricao, Ativo)
    VALUES
        (1, N'Administrador', 1),
        (2, N'Operador', 1),
        (3, N'Consulta', 1);

    SET IDENTITY_INSERT dbo.Perfil OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Operacao WHERE Id IN (1, 2))
BEGIN
    SET IDENTITY_INSERT dbo.Operacao ON;

    INSERT INTO dbo.Operacao (Id, Descricao, Ativo)
    VALUES
        (1, N'Inbound', 1),
        (2, N'Outbound', 1);

    SET IDENTITY_INSERT dbo.Operacao OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Status WHERE Id IN (1, 2, 3, 4, 5))
BEGIN
    SET IDENTITY_INSERT dbo.Status ON;

    INSERT INTO dbo.Status (Id, Descricao, Ativo)
    VALUES
        (1, N'Agendado', 1),
        (2, N'CheckInRealizado', 1),
        (3, N'EmDoca', 1),
        (4, N'Finalizado', 1),
        (5, N'Cancelado', 1);

    SET IDENTITY_INSERT dbo.Status OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ConfiguracaoAgendamento WHERE Ativo = 1)
BEGIN
    INSERT INTO dbo.ConfiguracaoAgendamento (IntervaloMinutos, HoraInicio, HoraFim, Ativo)
    VALUES (30, '08:00', '18:00', 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Email = N'admin@zyx.local' AND Ativo = 1)
BEGIN
    INSERT INTO dbo.Usuario (Nome, Email, Senha, PerfilId)
    VALUES (N'Administrador', N'admin@zyx.local', NULL, 1);
END
GO

MERGE dbo.Permissao AS alvo
USING
(
    VALUES
        (N'usuarios.visualizar', N'Visualizar usuarios'),
        (N'usuarios.criar', N'Criar usuarios'),
        (N'usuarios.editar', N'Editar usuarios'),
        (N'usuarios.excluir', N'Excluir usuarios'),
        (N'perfis.visualizar', N'Visualizar perfis'),
        (N'perfis.editar_permissoes', N'Editar permissoes dos perfis'),
        (N'transportadoras.visualizar', N'Visualizar transportadoras'),
        (N'transportadoras.criar', N'Criar transportadoras'),
        (N'transportadoras.editar', N'Editar transportadoras'),
        (N'transportadoras.excluir', N'Excluir transportadoras'),
        (N'veiculos.visualizar', N'Visualizar veiculos'),
        (N'veiculos.criar', N'Criar veiculos'),
        (N'veiculos.editar', N'Editar veiculos'),
        (N'veiculos.excluir', N'Excluir veiculos'),
        (N'motoristas.visualizar', N'Visualizar motoristas'),
        (N'motoristas.criar', N'Criar motoristas'),
        (N'motoristas.editar', N'Editar motoristas'),
        (N'motoristas.excluir', N'Excluir motoristas'),
        (N'produtos.visualizar', N'Visualizar produtos'),
        (N'produtos.criar', N'Criar produtos'),
        (N'produtos.editar', N'Editar produtos'),
        (N'produtos.excluir', N'Excluir produtos'),
        (N'locais.visualizar', N'Visualizar locais'),
        (N'locais.criar', N'Criar locais'),
        (N'locais.editar', N'Editar locais'),
        (N'locais.excluir', N'Excluir locais'),
        (N'agendamentos.visualizar', N'Visualizar agendamentos'),
        (N'agendamentos.criar', N'Criar agendamentos'),
        (N'agendamentos.editar', N'Editar agendamentos'),
        (N'agendamentos.cancelar', N'Cancelar agendamentos'),
        (N'checkin.realizar', N'Realizar check-in'),
        (N'operacoes.visualizar', N'Visualizar operacoes'),
        (N'operacoes.enviar_doca', N'Enviar operacao para doca'),
        (N'operacoes.finalizar', N'Finalizar operacao'),
        (N'inventario.visualizar', N'Visualizar inventario'),
        (N'configuracoes.visualizar', N'Visualizar configuracoes'),
        (N'configuracoes.editar', N'Editar configuracoes'),
        (N'relatorios.visualizar', N'Visualizar relatorios')
) AS origem (Codigo, Descricao)
ON alvo.Codigo = origem.Codigo
WHEN MATCHED THEN
    UPDATE SET
        alvo.Descricao = origem.Descricao,
        alvo.Ativo = 1
WHEN NOT MATCHED THEN
    INSERT (Codigo, Descricao, Ativo)
    VALUES (origem.Codigo, origem.Descricao, 1);
GO

INSERT INTO dbo.PerfilPermissao (PerfilId, PermissaoId)
SELECT 1, p.Id
FROM dbo.Permissao p
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.PerfilPermissao pp
    WHERE pp.PerfilId = 1
      AND pp.PermissaoId = p.Id
);
GO

INSERT INTO dbo.PerfilPermissao (PerfilId, PermissaoId)
SELECT 2, p.Id
FROM dbo.Permissao p
WHERE p.Codigo IN
(
    N'transportadoras.visualizar',
    N'veiculos.visualizar',
    N'veiculos.criar',
    N'veiculos.editar',
    N'motoristas.visualizar',
    N'motoristas.criar',
    N'motoristas.editar',
    N'produtos.visualizar',
    N'produtos.criar',
    N'produtos.editar',
    N'locais.visualizar',
    N'locais.criar',
    N'locais.editar',
    N'locais.excluir',
    N'agendamentos.visualizar',
    N'agendamentos.criar',
    N'agendamentos.editar',
    N'agendamentos.cancelar',
    N'checkin.realizar',
    N'operacoes.visualizar',
    N'operacoes.enviar_doca',
    N'operacoes.finalizar',
    N'inventario.visualizar',
    N'relatorios.visualizar'
)
AND NOT EXISTS
(
    SELECT 1
    FROM dbo.PerfilPermissao pp
    WHERE pp.PerfilId = 2
      AND pp.PermissaoId = p.Id
);
GO

INSERT INTO dbo.PerfilPermissao (PerfilId, PermissaoId)
SELECT 3, p.Id
FROM dbo.Permissao p
WHERE p.Codigo IN
(
    N'transportadoras.visualizar',
    N'veiculos.visualizar',
    N'motoristas.visualizar',
    N'produtos.visualizar',
    N'locais.visualizar',
    N'agendamentos.visualizar',
    N'operacoes.visualizar',
    N'inventario.visualizar',
    N'relatorios.visualizar'
)
AND NOT EXISTS
(
    SELECT 1
    FROM dbo.PerfilPermissao pp
    WHERE pp.PerfilId = 3
      AND pp.PermissaoId = p.Id
);
GO

CREATE OR ALTER PROCEDURE dbo.sp_Perfil_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Descricao, Ativo
    FROM dbo.Perfil
    WHERE Ativo = 1
    ORDER BY Descricao;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Perfil_ObterPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Descricao, Ativo
    FROM dbo.Perfil
    WHERE Id = @Id
      AND Ativo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Perfil_Inserir
    @Descricao NVARCHAR(100),
    @Permissoes dbo.IntIdList READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF EXISTS (SELECT 1 FROM dbo.Perfil WHERE Descricao = @Descricao AND Ativo = 1)
    BEGIN
        THROW 50005, 'Ja existe um perfil ativo com esta descricao.', 1;
    END

    IF EXISTS
    (
        SELECT 1
        FROM @Permissoes ids
        LEFT JOIN dbo.Permissao p ON p.Id = ids.Id AND p.Ativo = 1
        WHERE p.Id IS NULL
    )
    BEGIN
        THROW 50004, 'Uma ou mais permissoes informadas nao existem ou estao inativas.', 1;
    END

    BEGIN TRANSACTION;

    INSERT INTO dbo.Perfil (Descricao)
    VALUES (@Descricao);

    DECLARE @PerfilId INT = CAST(SCOPE_IDENTITY() AS INT);

    INSERT INTO dbo.PerfilPermissao (PerfilId, PermissaoId)
    SELECT @PerfilId, Id
    FROM @Permissoes;

    COMMIT TRANSACTION;

    SELECT Id, Descricao, Ativo
    FROM dbo.Perfil
    WHERE Id = @PerfilId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Perfil_Atualizar
    @Id INT,
    @Descricao NVARCHAR(100),
    @Permissoes dbo.IntIdList READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Perfil WHERE Id = @Id AND Ativo = 1)
    BEGIN
        THROW 50004, 'Perfil nao encontrado ou inativo.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.Perfil WHERE Descricao = @Descricao AND Id <> @Id AND Ativo = 1)
    BEGIN
        THROW 50005, 'Ja existe um perfil ativo com esta descricao.', 1;
    END

    IF EXISTS
    (
        SELECT 1
        FROM @Permissoes ids
        LEFT JOIN dbo.Permissao p ON p.Id = ids.Id AND p.Ativo = 1
        WHERE p.Id IS NULL
    )
    BEGIN
        THROW 50004, 'Uma ou mais permissoes informadas nao existem ou estao inativas.', 1;
    END

    BEGIN TRANSACTION;

    UPDATE dbo.Perfil
    SET Descricao = @Descricao
    WHERE Id = @Id
      AND Ativo = 1;

    DELETE FROM dbo.PerfilPermissao
    WHERE PerfilId = @Id;

    INSERT INTO dbo.PerfilPermissao (PerfilId, PermissaoId)
    SELECT @Id, Id
    FROM @Permissoes;

    COMMIT TRANSACTION;

    SELECT Id, Descricao, Ativo
    FROM dbo.Perfil
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Perfil_Excluir
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE PerfilId = @Id AND Ativo = 1)
    BEGIN
        THROW 50006, 'Nao e possivel excluir um perfil vinculado a usuarios ativos.', 1;
    END

    BEGIN TRANSACTION;

    DELETE FROM dbo.PerfilPermissao
    WHERE PerfilId = @Id;

    UPDATE dbo.Perfil
    SET Ativo = 0
    WHERE Id = @Id
      AND Ativo = 1;

    DECLARE @LinhasAfetadas INT = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @LinhasAfetadas AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Listar
    @Nome NVARCHAR(150) = NULL,
    @Email NVARCHAR(150) = NULL,
    @PerfilId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id,
        u.Nome,
        u.Email,
        u.PerfilId,
        p.Descricao AS PerfilDescricao,
        CAST(CASE WHEN u.Senha IS NULL THEN 1 ELSE 0 END AS BIT) AS PrimeiroAcesso,
        u.Ativo,
        u.CriadoEm,
        u.AtualizadoEm
    FROM dbo.Usuario u
    INNER JOIN dbo.Perfil p ON p.Id = u.PerfilId
    WHERE u.Ativo = 1
      AND p.Ativo = 1
      AND (@Nome IS NULL OR u.Nome LIKE '%' + @Nome + '%')
      AND (@Email IS NULL OR u.Email LIKE '%' + @Email + '%')
      AND (@PerfilId IS NULL OR u.PerfilId = @PerfilId)
    ORDER BY u.Nome;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Usuario_ObterPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id,
        u.Nome,
        u.Email,
        u.PerfilId,
        p.Descricao AS PerfilDescricao,
        CAST(CASE WHEN u.Senha IS NULL THEN 1 ELSE 0 END AS BIT) AS PrimeiroAcesso,
        u.Ativo,
        u.CriadoEm,
        u.AtualizadoEm
    FROM dbo.Usuario u
    INNER JOIN dbo.Perfil p ON p.Id = u.PerfilId
    WHERE u.Id = @Id
      AND u.Ativo = 1
      AND p.Ativo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Inserir
    @Nome NVARCHAR(150),
    @Email NVARCHAR(150),
    @PerfilId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Perfil WHERE Id = @PerfilId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Perfil nao encontrado ou inativo.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Email = @Email AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe um usuario ativo com este email.', 1;
    END

    INSERT INTO dbo.Usuario (Nome, Email, Senha, PerfilId)
    VALUES (@Nome, @Email, NULL, @PerfilId);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Atualizar
    @Id INT,
    @Nome NVARCHAR(150),
    @Email NVARCHAR(150),
    @PerfilId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Perfil WHERE Id = @PerfilId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Perfil nao encontrado ou inativo.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Email = @Email AND Id <> @Id AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe um usuario ativo com este email.', 1;
    END

    UPDATE dbo.Usuario
    SET
        Nome = @Nome,
        Email = @Email,
        PerfilId = @PerfilId,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Excluir
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Usuario
    SET
        Ativo = 0,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Usuario_VerificarPrimeiroAcesso
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id,
        u.Nome,
        u.Email,
        u.PerfilId,
        p.Descricao AS PerfilDescricao,
        CAST(CASE WHEN u.Senha IS NULL THEN 1 ELSE 0 END AS BIT) AS PrimeiroAcesso,
        u.Ativo
    FROM dbo.Usuario u
    INNER JOIN dbo.Perfil p ON p.Id = u.PerfilId
    WHERE u.Email = @Email
      AND u.Ativo = 1
      AND p.Ativo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Usuario_DefinirSenhaPrimeiroAcesso
    @Email NVARCHAR(150),
    @Senha NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Usuario
    SET
        Senha = @Senha,
        AtualizadoEm = SYSDATETIME()
    WHERE Email = @Email
      AND Ativo = 1
      AND Senha IS NULL;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Login
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id,
        u.Nome,
        u.Email,
        u.Senha,
        u.PerfilId,
        p.Descricao AS PerfilDescricao,
        CAST(CASE WHEN u.Senha IS NULL THEN 1 ELSE 0 END AS BIT) AS PrimeiroAcesso,
        u.Ativo
    FROM dbo.Usuario u
    INNER JOIN dbo.Perfil p ON p.Id = u.PerfilId
    WHERE u.Email = @Email
      AND u.Ativo = 1
      AND p.Ativo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Usuario_ListarPermissoes
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Id,
        p.Codigo,
        p.Descricao,
        p.Ativo
    FROM dbo.Usuario u
    INNER JOIN dbo.PerfilPermissao pp ON pp.PerfilId = u.PerfilId
    INNER JOIN dbo.Permissao p ON p.Id = pp.PermissaoId
    WHERE u.Id = @UsuarioId
      AND u.Ativo = 1
      AND p.Ativo = 1
    ORDER BY p.Codigo;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Permissao_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Codigo, Descricao, Ativo
    FROM dbo.Permissao
    WHERE Ativo = 1
    ORDER BY Codigo;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Permissao_ListarPorPerfil
    @PerfilId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Id,
        p.Codigo,
        p.Descricao,
        p.Ativo
    FROM dbo.PerfilPermissao pp
    INNER JOIN dbo.Permissao p ON p.Id = pp.PermissaoId
    WHERE pp.PerfilId = @PerfilId
      AND p.Ativo = 1
    ORDER BY p.Codigo;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Perfil_AtualizarPermissoes
    @PerfilId INT,
    @Permissoes dbo.IntIdList READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Perfil WHERE Id = @PerfilId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Perfil nao encontrado ou inativo.', 1;
    END

    IF EXISTS
    (
        SELECT 1
        FROM @Permissoes ids
        LEFT JOIN dbo.Permissao p ON p.Id = ids.Id AND p.Ativo = 1
        WHERE p.Id IS NULL
    )
    BEGIN
        THROW 50004, 'Uma ou mais permissoes informadas nao existem ou estao inativas.', 1;
    END

    BEGIN TRANSACTION;

    DELETE FROM dbo.PerfilPermissao
    WHERE PerfilId = @PerfilId;

    INSERT INTO dbo.PerfilPermissao (PerfilId, PermissaoId)
    SELECT @PerfilId, Id
    FROM @Permissoes;

    COMMIT TRANSACTION;

    SELECT
        p.Id,
        p.Codigo,
        p.Descricao,
        p.Ativo
    FROM dbo.PerfilPermissao pp
    INNER JOIN dbo.Permissao p ON p.Id = pp.PermissaoId
    WHERE pp.PerfilId = @PerfilId
      AND p.Ativo = 1
    ORDER BY p.Codigo;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Operacao_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Descricao, Ativo
    FROM dbo.Operacao
    WHERE Ativo = 1
    ORDER BY Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Status_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Descricao, Ativo
    FROM dbo.Status
    WHERE Ativo = 1
    ORDER BY Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Operacao_ListarAbas
    @OperacaoId INT,
    @Busca NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Operacao WHERE Id = @OperacaoId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Operacao nao encontrada ou inativa.', 1;
    END

    SELECT
        a.Id,
        a.OperacaoId,
        o.Descricao AS OperacaoDescricao,
        a.StatusId,
        s.Descricao AS StatusDescricao,
        a.VeiculoId,
        v.Placa AS VeiculoPlaca,
        v.TipoVeiculo,
        a.TransportadoraId,
        t.Nome AS TransportadoraNome,
        a.LocalId,
        l.Descricao AS LocalDescricao,
        a.MotoristaId,
        m.Nome AS MotoristaNome,
        m.Cnh AS MotoristaCnh,
        a.DataHoraAgendada,
        a.DataHoraChegada,
        a.DataHoraDoca,
        a.DataHoraFinalizado,
        a.CriadoEm,
        a.AtualizadoEm
    FROM dbo.Agendamento a
    INNER JOIN dbo.Operacao o ON o.Id = a.OperacaoId
    INNER JOIN dbo.Status s ON s.Id = a.StatusId
    INNER JOIN dbo.Veiculo v ON v.Id = a.VeiculoId
    INNER JOIN dbo.Transportadora t ON t.Id = a.TransportadoraId
    LEFT JOIN dbo.Local l ON l.Id = a.LocalId
    INNER JOIN dbo.Motorista m ON m.Id = a.MotoristaId
    WHERE a.OperacaoId = @OperacaoId
      AND a.StatusId IN (2, 3, 4)
      AND
      (
          @Busca IS NULL
          OR CAST(a.Id AS NVARCHAR(20)) LIKE '%' + @Busca + '%'
          OR v.Placa LIKE '%' + @Busca + '%'
          OR t.Nome LIKE '%' + @Busca + '%'
          OR m.Nome LIKE '%' + @Busca + '%'
      )
    ORDER BY
        CASE a.StatusId
            WHEN 2 THEN 1
            WHEN 3 THEN 2
            WHEN 4 THEN 3
            ELSE 4
        END,
        a.DataHoraAgendada,
        a.Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ConfiguracaoAgendamento_ObterAtiva
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        Id,
        IntervaloMinutos,
        HoraInicio,
        HoraFim,
        Ativo,
        CriadoEm,
        AtualizadoEm
    FROM dbo.ConfiguracaoAgendamento
    WHERE Ativo = 1
    ORDER BY Id DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ConfiguracaoAgendamento_Atualizar
    @IntervaloMinutos INT,
    @HoraInicio TIME(0),
    @HoraFim TIME(0)
AS
BEGIN
    SET NOCOUNT ON;

    IF @IntervaloMinutos <= 0
    BEGIN
        THROW 50003, 'O intervalo de agendamento deve ser maior que zero.', 1;
    END

    IF @HoraInicio >= @HoraFim
    BEGIN
        THROW 50003, 'A hora inicial deve ser menor que a hora final.', 1;
    END

    DECLARE @Id INT;

    SELECT TOP (1) @Id = Id
    FROM dbo.ConfiguracaoAgendamento
    WHERE Ativo = 1
    ORDER BY Id DESC;

    IF @Id IS NULL
    BEGIN
        INSERT INTO dbo.ConfiguracaoAgendamento (IntervaloMinutos, HoraInicio, HoraFim, Ativo)
        VALUES (@IntervaloMinutos, @HoraInicio, @HoraFim, 1);

        SET @Id = CAST(SCOPE_IDENTITY() AS INT);
    END
    ELSE
    BEGIN
        UPDATE dbo.ConfiguracaoAgendamento
        SET
            IntervaloMinutos = @IntervaloMinutos,
            HoraInicio = @HoraInicio,
            HoraFim = @HoraFim,
            AtualizadoEm = SYSDATETIME()
        WHERE Id = @Id;
    END

    SELECT
        Id,
        IntervaloMinutos,
        HoraInicio,
        HoraFim,
        Ativo,
        CriadoEm,
        AtualizadoEm
    FROM dbo.ConfiguracaoAgendamento
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Agendamento_Listar
    @Data DATE,
    @OperacaoId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DataInicio DATETIME2(0) = CAST(@Data AS DATETIME2(0));
    DECLARE @DataFim DATETIME2(0) = DATEADD(DAY, 7, @DataInicio);

    IF NOT EXISTS (SELECT 1 FROM dbo.Operacao WHERE Id = @OperacaoId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Operacao nao encontrada ou inativa.', 1;
    END

    SELECT
        a.Id,
        a.OperacaoId,
        o.Descricao AS OperacaoDescricao,
        a.StatusId,
        s.Descricao AS StatusDescricao,
        a.VeiculoId,
        v.Placa AS VeiculoPlaca,
        v.TipoVeiculo,
        a.TransportadoraId,
        t.Nome AS TransportadoraNome,
        a.LocalId,
        l.Descricao AS LocalDescricao,
        a.MotoristaId,
        m.Nome AS MotoristaNome,
        m.Cnh AS MotoristaCnh,
        a.DataHoraAgendada,
        a.DataHoraChegada,
        a.DataHoraDoca,
        a.DataHoraFinalizado,
        a.CriadoEm,
        a.AtualizadoEm
    FROM dbo.Agendamento a
    INNER JOIN dbo.Operacao o ON o.Id = a.OperacaoId
    INNER JOIN dbo.Status s ON s.Id = a.StatusId
    INNER JOIN dbo.Veiculo v ON v.Id = a.VeiculoId
    INNER JOIN dbo.Transportadora t ON t.Id = a.TransportadoraId
    LEFT JOIN dbo.Local l ON l.Id = a.LocalId
    INNER JOIN dbo.Motorista m ON m.Id = a.MotoristaId
    WHERE a.DataHoraAgendada >= @DataInicio
      AND a.DataHoraAgendada < @DataFim
      AND a.OperacaoId = @OperacaoId
      AND a.StatusId IN (1, 2, 3, 4)
    ORDER BY a.DataHoraAgendada, a.Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Agendamento_ObterPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.Id,
        a.OperacaoId,
        o.Descricao AS OperacaoDescricao,
        a.StatusId,
        s.Descricao AS StatusDescricao,
        a.VeiculoId,
        v.Placa AS VeiculoPlaca,
        v.TipoVeiculo,
        a.TransportadoraId,
        t.Nome AS TransportadoraNome,
        a.LocalId,
        l.Descricao AS LocalDescricao,
        a.MotoristaId,
        m.Nome AS MotoristaNome,
        m.Cnh AS MotoristaCnh,
        a.DataHoraAgendada,
        a.DataHoraChegada,
        a.DataHoraDoca,
        a.DataHoraFinalizado,
        a.CriadoEm,
        a.AtualizadoEm
    FROM dbo.Agendamento a
    INNER JOIN dbo.Operacao o ON o.Id = a.OperacaoId
    INNER JOIN dbo.Status s ON s.Id = a.StatusId
    INNER JOIN dbo.Veiculo v ON v.Id = a.VeiculoId
    INNER JOIN dbo.Transportadora t ON t.Id = a.TransportadoraId
    LEFT JOIN dbo.Local l ON l.Id = a.LocalId
    INNER JOIN dbo.Motorista m ON m.Id = a.MotoristaId
    WHERE a.Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Agendamento_ListarHorariosDisponiveis
    @Data DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IntervaloMinutos INT;
    DECLARE @HoraInicio TIME(0);
    DECLARE @HoraFim TIME(0);
    DECLARE @CapacidadeLocais INT;

    SELECT TOP (1)
        @IntervaloMinutos = IntervaloMinutos,
        @HoraInicio = HoraInicio,
        @HoraFim = HoraFim
    FROM dbo.ConfiguracaoAgendamento
    WHERE Ativo = 1
    ORDER BY Id DESC;

    IF @IntervaloMinutos IS NULL
    BEGIN
        THROW 50008, 'Configuracao de agendamento ativa nao encontrada.', 1;
    END

    SELECT @CapacidadeLocais = COUNT(1)
    FROM dbo.Local
    WHERE Ativo = 1;

    IF @CapacidadeLocais <= 0
    BEGIN
        THROW 50008, 'Nenhum local ativo cadastrado para permitir agendamentos.', 1;
    END

    ;WITH Horarios AS
    (
        SELECT DATEADD(MINUTE, DATEDIFF(MINUTE, CAST('00:00' AS TIME), @HoraInicio), CAST(@Data AS DATETIME2(0))) AS DataHora
        UNION ALL
        SELECT DATEADD(MINUTE, @IntervaloMinutos, DataHora)
        FROM Horarios
        WHERE DATEADD(MINUTE, @IntervaloMinutos, DataHora) < DATEADD(MINUTE, DATEDIFF(MINUTE, CAST('00:00' AS TIME), @HoraFim), CAST(@Data AS DATETIME2(0)))
    )
    SELECT
        h.DataHora,
        CONVERT(VARCHAR(5), CAST(h.DataHora AS TIME(0)), 108) AS Horario
    FROM Horarios h
    WHERE
    (
        SELECT COUNT(1)
        FROM dbo.Agendamento a
        WHERE a.DataHoraAgendada = h.DataHora
          AND a.StatusId IN (1, 2, 3)
    ) < @CapacidadeLocais
    ORDER BY h.DataHora
    OPTION (MAXRECURSION 1440);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Agendamento_ListarVeiculosDisponiveis
    @TransportadoraId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Transportadora WHERE Id = @TransportadoraId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Transportadora nao encontrada ou inativa.', 1;
    END

    SELECT
        v.Id,
        v.Placa,
        v.TipoVeiculo,
        v.TransportadoraId,
        t.Nome AS TransportadoraNome
    FROM dbo.Veiculo v
    INNER JOIN dbo.Transportadora t ON t.Id = v.TransportadoraId
    WHERE v.TransportadoraId = @TransportadoraId
      AND v.Ativo = 1
      AND t.Ativo = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.Agendamento a
          WHERE a.VeiculoId = v.Id
            AND a.StatusId IN (1, 2, 3)
      )
    ORDER BY v.Placa;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Agendamento_ListarMotoristasDisponiveis
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.Id,
        m.Nome,
        m.Cnh,
        m.Telefone
    FROM dbo.Motorista m
    WHERE m.Ativo = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.Agendamento a
          WHERE a.MotoristaId = m.Id
            AND a.StatusId IN (1, 2, 3)
      )
    ORDER BY m.Nome;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Agendamento_Inserir
    @OperacaoId INT,
    @TransportadoraId INT,
    @VeiculoId INT,
    @MotoristaId INT,
    @DataHoraAgendada DATETIME2(0)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IntervaloMinutos INT;
    DECLARE @HoraInicio TIME(0);
    DECLARE @HoraFim TIME(0);
    DECLARE @HoraAgendada TIME(0) = CAST(@DataHoraAgendada AS TIME(0));
    DECLARE @MinutosDesdeInicio INT;
    DECLARE @CapacidadeLocais INT;

    SELECT TOP (1)
        @IntervaloMinutos = IntervaloMinutos,
        @HoraInicio = HoraInicio,
        @HoraFim = HoraFim
    FROM dbo.ConfiguracaoAgendamento
    WHERE Ativo = 1
    ORDER BY Id DESC;

    IF @IntervaloMinutos IS NULL
    BEGIN
        THROW 50008, 'Configuracao de agendamento ativa nao encontrada.', 1;
    END

    SELECT @CapacidadeLocais = COUNT(1)
    FROM dbo.Local
    WHERE Ativo = 1;

    IF @CapacidadeLocais <= 0
    BEGIN
        THROW 50008, 'Nenhum local ativo cadastrado para permitir agendamentos.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Operacao WHERE Id = @OperacaoId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Operacao nao encontrada ou inativa.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Transportadora WHERE Id = @TransportadoraId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Transportadora nao encontrada ou inativa.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Veiculo WHERE Id = @VeiculoId AND TransportadoraId = @TransportadoraId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Veiculo nao encontrado, inativo ou nao pertence a transportadora informada.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Motorista WHERE Id = @MotoristaId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Motorista nao encontrado ou inativo.', 1;
    END

    IF @HoraAgendada < @HoraInicio OR @HoraAgendada >= @HoraFim
    BEGIN
        THROW 50007, 'Horario fora da janela de agendamento.', 1;
    END

    SET @MinutosDesdeInicio = DATEDIFF(MINUTE, @HoraInicio, @HoraAgendada);

    IF @MinutosDesdeInicio % @IntervaloMinutos <> 0
    BEGIN
        THROW 50007, 'Horario nao respeita o intervalo configurado.', 1;
    END

    IF (SELECT COUNT(1) FROM dbo.Agendamento WHERE DataHoraAgendada = @DataHoraAgendada AND StatusId IN (1, 2, 3)) >= @CapacidadeLocais
    BEGIN
        THROW 50007, 'Horario ja atingiu a capacidade de locais disponiveis.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.Agendamento WHERE MotoristaId = @MotoristaId AND StatusId IN (1, 2, 3))
    BEGIN
        THROW 50007, 'Motorista ja possui agendamento ativo.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.Agendamento WHERE VeiculoId = @VeiculoId AND StatusId IN (1, 2, 3))
    BEGIN
        THROW 50007, 'Veiculo ja possui agendamento ativo.', 1;
    END

    INSERT INTO dbo.Agendamento (OperacaoId, StatusId, TransportadoraId, VeiculoId, MotoristaId, DataHoraAgendada)
    VALUES (@OperacaoId, 1, @TransportadoraId, @VeiculoId, @MotoristaId, @DataHoraAgendada);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Agendamento_Atualizar
    @Id INT,
    @OperacaoId INT,
    @TransportadoraId INT,
    @VeiculoId INT,
    @MotoristaId INT,
    @DataHoraAgendada DATETIME2(0)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IntervaloMinutos INT;
    DECLARE @HoraInicio TIME(0);
    DECLARE @HoraFim TIME(0);
    DECLARE @HoraAgendada TIME(0) = CAST(@DataHoraAgendada AS TIME(0));
    DECLARE @MinutosDesdeInicio INT;
    DECLARE @CapacidadeLocais INT;

    IF NOT EXISTS (SELECT 1 FROM dbo.Agendamento WHERE Id = @Id AND StatusId = 1)
    BEGIN
        THROW 50007, 'Agendamento nao encontrado ou nao pode mais ser editado.', 1;
    END

    SELECT TOP (1)
        @IntervaloMinutos = IntervaloMinutos,
        @HoraInicio = HoraInicio,
        @HoraFim = HoraFim
    FROM dbo.ConfiguracaoAgendamento
    WHERE Ativo = 1
    ORDER BY Id DESC;

    IF @IntervaloMinutos IS NULL
    BEGIN
        THROW 50008, 'Configuracao de agendamento ativa nao encontrada.', 1;
    END

    SELECT @CapacidadeLocais = COUNT(1)
    FROM dbo.Local
    WHERE Ativo = 1;

    IF @CapacidadeLocais <= 0
    BEGIN
        THROW 50008, 'Nenhum local ativo cadastrado para permitir agendamentos.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Operacao WHERE Id = @OperacaoId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Operacao nao encontrada ou inativa.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Transportadora WHERE Id = @TransportadoraId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Transportadora nao encontrada ou inativa.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Veiculo WHERE Id = @VeiculoId AND TransportadoraId = @TransportadoraId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Veiculo nao encontrado, inativo ou nao pertence a transportadora informada.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Motorista WHERE Id = @MotoristaId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Motorista nao encontrado ou inativo.', 1;
    END

    IF @HoraAgendada < @HoraInicio OR @HoraAgendada >= @HoraFim
    BEGIN
        THROW 50007, 'Horario fora da janela de agendamento.', 1;
    END

    SET @MinutosDesdeInicio = DATEDIFF(MINUTE, @HoraInicio, @HoraAgendada);

    IF @MinutosDesdeInicio % @IntervaloMinutos <> 0
    BEGIN
        THROW 50007, 'Horario nao respeita o intervalo configurado.', 1;
    END

    IF (SELECT COUNT(1) FROM dbo.Agendamento WHERE Id <> @Id AND DataHoraAgendada = @DataHoraAgendada AND StatusId IN (1, 2, 3)) >= @CapacidadeLocais
    BEGIN
        THROW 50007, 'Horario ja atingiu a capacidade de locais disponiveis.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.Agendamento WHERE Id <> @Id AND MotoristaId = @MotoristaId AND StatusId IN (1, 2, 3))
    BEGIN
        THROW 50007, 'Motorista ja possui agendamento ativo.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.Agendamento WHERE Id <> @Id AND VeiculoId = @VeiculoId AND StatusId IN (1, 2, 3))
    BEGIN
        THROW 50007, 'Veiculo ja possui agendamento ativo.', 1;
    END

    UPDATE dbo.Agendamento
    SET
        OperacaoId = @OperacaoId,
        TransportadoraId = @TransportadoraId,
        VeiculoId = @VeiculoId,
        MotoristaId = @MotoristaId,
        DataHoraAgendada = @DataHoraAgendada,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND StatusId = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.sp_Agendamento_Cancelar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Agendamento WHERE Id = @Id AND StatusId = 4)
    BEGIN
        THROW 50007, 'Agenda finalizada nao pode ser cancelada.', 1;
    END

    UPDATE dbo.Agendamento
    SET
        StatusId = 5,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND StatusId IN (1, 2, 3);

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CheckIn_SolicitarCodigo
    @Cnh VARCHAR(30),
    @Codigo VARCHAR(6)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @MotoristaId INT;
    DECLARE @MotoristaNome NVARCHAR(150);
    DECLARE @Telefone VARCHAR(20);
    DECLARE @AgendamentoId INT;
    DECLARE @ExpiraEm DATETIME2(0) = DATEADD(MINUTE, 10, SYSDATETIME());
    DECLARE @TelefoneMascarado VARCHAR(20);
    DECLARE @Mensagem NVARCHAR(300);

    SELECT
        @MotoristaId = Id,
        @MotoristaNome = Nome,
        @Telefone = Telefone
    FROM dbo.Motorista
    WHERE Cnh = @Cnh
      AND Ativo = 1;

    IF @MotoristaId IS NULL
    BEGIN
        THROW 50004, 'Motorista nao encontrado para esta CNH.', 1;
    END

    IF NULLIF(LTRIM(RTRIM(@Telefone)), '') IS NULL
    BEGIN
        THROW 50007, 'Motorista nao possui telefone cadastrado.', 1;
    END

    SELECT TOP (1) @AgendamentoId = Id
    FROM dbo.Agendamento
    WHERE MotoristaId = @MotoristaId
      AND StatusId = 1
      AND DataHoraAgendada >= DATEADD(DAY, -1, SYSDATETIME())
    ORDER BY DataHoraAgendada;

    IF @AgendamentoId IS NULL
    BEGIN
        THROW 50004, 'Agendamento ativo nao encontrado para este motorista.', 1;
    END

    UPDATE dbo.CheckInCodigo
    SET UsadoEm = SYSDATETIME()
    WHERE AgendamentoId = @AgendamentoId
      AND UsadoEm IS NULL;

    SET @Mensagem = N'Codigo de check-in ZYX: ' + @Codigo;
    SET @TelefoneMascarado =
        CASE
            WHEN LEN(@Telefone) <= 4 THEN REPLICATE('*', LEN(@Telefone))
            ELSE REPLICATE('*', LEN(@Telefone) - 4) + RIGHT(@Telefone, 4)
        END;

    BEGIN TRANSACTION;

        INSERT INTO dbo.CheckInCodigo (AgendamentoId, MotoristaId, Cnh, Telefone, Codigo, ExpiraEm)
        VALUES (@AgendamentoId, @MotoristaId, @Cnh, @Telefone, @Codigo, @ExpiraEm);

        INSERT INTO dbo.SmsEnvio (Telefone, Mensagem)
        VALUES (@Telefone, @Mensagem);

    COMMIT TRANSACTION;

    SELECT
        @AgendamentoId AS AgendamentoId,
        @MotoristaNome AS MotoristaNome,
        @TelefoneMascarado AS TelefoneMascarado,
        @ExpiraEm AS ExpiraEm,
        @Codigo AS CodigoDesenvolvimento,
        @Telefone AS Telefone,
        @Mensagem AS Mensagem;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CheckIn_Confirmar
    @Cnh VARCHAR(30),
    @Codigo VARCHAR(6)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @CheckInCodigoId INT;
    DECLARE @AgendamentoId INT;
    DECLARE @LinhasAfetadas INT = 0;

    SELECT TOP (1)
        @CheckInCodigoId = cic.Id,
        @AgendamentoId = cic.AgendamentoId
    FROM dbo.CheckInCodigo cic
    INNER JOIN dbo.Agendamento a ON a.Id = cic.AgendamentoId
    WHERE cic.Cnh = @Cnh
      AND cic.Codigo = @Codigo
      AND cic.UsadoEm IS NULL
      AND cic.ExpiraEm >= SYSDATETIME()
      AND a.StatusId = 1
    ORDER BY cic.Id DESC;

    IF @CheckInCodigoId IS NULL
    BEGIN
        THROW 50007, 'Codigo invalido, expirado ou agendamento nao esta mais aguardando check-in.', 1;
    END

    BEGIN TRANSACTION;

        UPDATE dbo.Agendamento
        SET
            StatusId = 2,
            DataHoraChegada = SYSDATETIME(),
            AtualizadoEm = SYSDATETIME()
        WHERE Id = @AgendamentoId
          AND StatusId = 1;

        SET @LinhasAfetadas = @@ROWCOUNT;

        UPDATE dbo.CheckInCodigo
        SET UsadoEm = SYSDATETIME()
        WHERE Id = @CheckInCodigoId;

    COMMIT TRANSACTION;

    SELECT @LinhasAfetadas AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Agendamento_EnviarDoca
    @Id INT,
    @LocalId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Local WHERE Id = @LocalId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Local nao encontrado ou inativo.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.Agendamento WHERE LocalId = @LocalId AND StatusId = 3 AND Id <> @Id)
    BEGIN
        THROW 50007, 'Local ja esta em uso por outro agendamento em doca.', 1;
    END

    UPDATE dbo.Agendamento
    SET
        StatusId = 3,
        LocalId = @LocalId,
        DataHoraDoca = SYSDATETIME(),
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND StatusId = 2;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Agendamento_Finalizar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Agendamento
    SET
        StatusId = 4,
        DataHoraFinalizado = SYSDATETIME(),
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND StatusId = 3;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Transportadora_Listar
    @Nome NVARCHAR(150) = NULL,
    @Cnpj VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Nome,
        Cnpj,
        Ativo,
        CriadoEm,
        AtualizadoEm
    FROM dbo.Transportadora
    WHERE Ativo = 1
      AND (@Nome IS NULL OR Nome LIKE '%' + @Nome + '%')
      AND (@Cnpj IS NULL OR Cnpj LIKE '%' + @Cnpj + '%')
    ORDER BY Nome;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Transportadora_ObterPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Nome,
        Cnpj,
        Ativo,
        CriadoEm,
        AtualizadoEm
    FROM dbo.Transportadora
    WHERE Id = @Id
      AND Ativo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Transportadora_Inserir
    @Nome NVARCHAR(150),
    @Cnpj VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Transportadora WHERE Cnpj = @Cnpj AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe uma transportadora cadastrada com este CNPJ.', 1;
    END

    INSERT INTO dbo.Transportadora (Nome, Cnpj)
    VALUES (@Nome, @Cnpj);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Transportadora_Atualizar
    @Id INT,
    @Nome NVARCHAR(150),
    @Cnpj VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Transportadora WHERE Cnpj = @Cnpj AND Id <> @Id AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe uma transportadora cadastrada com este CNPJ.', 1;
    END

    UPDATE dbo.Transportadora
    SET
        Nome = @Nome,
        Cnpj = @Cnpj,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Transportadora_Excluir
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Transportadora
    SET
        Ativo = 0,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Local_Listar
    @Descricao NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Descricao,
        Ativo,
        CriadoEm,
        AtualizadoEm
    FROM dbo.Local
    WHERE Ativo = 1
      AND (@Descricao IS NULL OR Descricao LIKE '%' + @Descricao + '%')
    ORDER BY Descricao;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Local_ObterPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Descricao,
        Ativo,
        CriadoEm,
        AtualizadoEm
    FROM dbo.Local
    WHERE Id = @Id
      AND Ativo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Local_Inserir
    @Descricao NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Local WHERE Descricao = @Descricao AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe um local cadastrado com esta descricao.', 1;
    END

    INSERT INTO dbo.Local (Descricao)
    VALUES (@Descricao);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Local_Atualizar
    @Id INT,
    @Descricao NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Local WHERE Descricao = @Descricao AND Id <> @Id AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe um local cadastrado com esta descricao.', 1;
    END

    UPDATE dbo.Local
    SET
        Descricao = @Descricao,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Local_Excluir
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Agendamento WHERE LocalId = @Id AND StatusId = 3)
    BEGIN
        THROW 50007, 'Nao e possivel excluir um local em uso por operacao ativa.', 1;
    END

    UPDATE dbo.Local
    SET
        Ativo = 0,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Motorista_Listar
    @Nome NVARCHAR(150) = NULL,
    @Cnh VARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Nome,
        Cnh,
        Telefone,
        Ativo,
        CriadoEm,
        AtualizadoEm
    FROM dbo.Motorista
    WHERE Ativo = 1
      AND (@Nome IS NULL OR Nome LIKE '%' + @Nome + '%')
      AND (@Cnh IS NULL OR Cnh LIKE '%' + @Cnh + '%')
    ORDER BY Nome;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Motorista_ObterPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Nome,
        Cnh,
        Telefone,
        Ativo,
        CriadoEm,
        AtualizadoEm
    FROM dbo.Motorista
    WHERE Id = @Id
      AND Ativo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Motorista_Inserir
    @Nome NVARCHAR(150),
    @Cnh VARCHAR(30),
    @Telefone VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Motorista WHERE Cnh = @Cnh AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe um motorista cadastrado com esta CNH.', 1;
    END

    INSERT INTO dbo.Motorista (Nome, Cnh, Telefone)
    VALUES (@Nome, @Cnh, @Telefone);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Motorista_Atualizar
    @Id INT,
    @Nome NVARCHAR(150),
    @Cnh VARCHAR(30),
    @Telefone VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Motorista WHERE Cnh = @Cnh AND Id <> @Id AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe um motorista cadastrado com esta CNH.', 1;
    END

    UPDATE dbo.Motorista
    SET
        Nome = @Nome,
        Cnh = @Cnh,
        Telefone = @Telefone,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Motorista_Excluir
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Motorista
    SET
        Ativo = 0,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Produto_Listar
    @Descricao NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Descricao,
        Ativo,
        CriadoEm,
        AtualizadoEm
    FROM dbo.Produto
    WHERE Ativo = 1
      AND (@Descricao IS NULL OR Descricao LIKE '%' + @Descricao + '%')
    ORDER BY Descricao;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Produto_ObterPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Descricao,
        Ativo,
        CriadoEm,
        AtualizadoEm
    FROM dbo.Produto
    WHERE Id = @Id
      AND Ativo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Produto_Inserir
    @Descricao NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ProdutoId INT;

    IF EXISTS (SELECT 1 FROM dbo.Produto WHERE Descricao = @Descricao AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe um produto cadastrado com esta descricao.', 1;
    END

    BEGIN TRANSACTION;

        INSERT INTO dbo.Produto (Descricao)
        VALUES (@Descricao);

        SET @ProdutoId = CAST(SCOPE_IDENTITY() AS INT);

        INSERT INTO dbo.Inventario (ProdutoId, QuantidadeAtual)
        VALUES (@ProdutoId, 0);

    COMMIT TRANSACTION;

    SELECT @ProdutoId AS Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Produto_Atualizar
    @Id INT,
    @Descricao NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Produto WHERE Descricao = @Descricao AND Id <> @Id AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe um produto cadastrado com esta descricao.', 1;
    END

    UPDATE dbo.Produto
    SET
        Descricao = @Descricao,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Produto_Excluir
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Produto
    SET
        Ativo = 0,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Inventario_Listar
    @Produto NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Id AS ProdutoId,
        p.Descricao AS ProdutoDescricao,
        i.QuantidadeAtual,
        i.DataUltimaAtualizacao
    FROM dbo.Inventario i
    INNER JOIN dbo.Produto p ON p.Id = i.ProdutoId
    WHERE p.Ativo = 1
      AND (@Produto IS NULL OR p.Descricao LIKE '%' + @Produto + '%')
    ORDER BY p.Descricao;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_OperacaoItem_ListarPorAgendamento
    @AgendamentoId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        oi.Id,
        oi.AgendamentoId,
        oi.ProdutoId,
        p.Descricao AS ProdutoDescricao,
        oi.Quantidade,
        oi.CriadoEm,
        oi.AtualizadoEm
    FROM dbo.OperacaoItem oi
    INNER JOIN dbo.Produto p ON p.Id = oi.ProdutoId
    WHERE oi.AgendamentoId = @AgendamentoId
      AND p.Ativo = 1
    ORDER BY p.Descricao;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_OperacaoItem_Inserir
    @AgendamentoId INT,
    @ProdutoId INT,
    @Quantidade DECIMAL(18,3)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @OperacaoId INT;
    DECLARE @StatusId INT;
    DECLARE @EstoqueAtual DECIMAL(18,3);
    DECLARE @ItemId INT;

    SELECT
        @OperacaoId = OperacaoId,
        @StatusId = StatusId
    FROM dbo.Agendamento
    WHERE Id = @AgendamentoId;

    IF @OperacaoId IS NULL
    BEGIN
        THROW 50004, 'Agendamento nao encontrado.', 1;
    END

    IF @StatusId <> 3
    BEGIN
        THROW 50007, 'Itens so podem ser alterados quando o agendamento esta em doca.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Produto WHERE Id = @ProdutoId AND Ativo = 1)
    BEGIN
        THROW 50004, 'Produto nao encontrado ou inativo.', 1;
    END

    BEGIN TRANSACTION;

        SELECT @ItemId = Id
        FROM dbo.OperacaoItem WITH (UPDLOCK, HOLDLOCK)
        WHERE AgendamentoId = @AgendamentoId
          AND ProdutoId = @ProdutoId;

        SELECT @EstoqueAtual = QuantidadeAtual
        FROM dbo.Inventario WITH (UPDLOCK, HOLDLOCK)
        WHERE ProdutoId = @ProdutoId;

        IF @EstoqueAtual IS NULL
        BEGIN
            INSERT INTO dbo.Inventario (ProdutoId, QuantidadeAtual)
            VALUES (@ProdutoId, 0);

            SET @EstoqueAtual = 0;
        END

        IF @OperacaoId = 2 AND @EstoqueAtual < @Quantidade
        BEGIN
            THROW 50007, 'Estoque insuficiente para carregar esta quantidade.', 1;
        END

        UPDATE dbo.Inventario
        SET
            QuantidadeAtual = CASE WHEN @OperacaoId = 1 THEN QuantidadeAtual + @Quantidade ELSE QuantidadeAtual - @Quantidade END,
            DataUltimaAtualizacao = SYSDATETIME()
        WHERE ProdutoId = @ProdutoId;

        IF @ItemId IS NULL
        BEGIN
            INSERT INTO dbo.OperacaoItem (AgendamentoId, ProdutoId, Quantidade)
            VALUES (@AgendamentoId, @ProdutoId, @Quantidade);

            SET @ItemId = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            UPDATE dbo.OperacaoItem
            SET
                Quantidade = Quantidade + @Quantidade,
                AtualizadoEm = SYSDATETIME()
            WHERE Id = @ItemId
              AND AgendamentoId = @AgendamentoId;
        END

    COMMIT TRANSACTION;

    SELECT @ItemId AS Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_OperacaoItem_Atualizar
    @AgendamentoId INT,
    @Id INT,
    @Quantidade DECIMAL(18,3)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @OperacaoId INT;
    DECLARE @StatusId INT;
    DECLARE @ProdutoId INT;
    DECLARE @QuantidadeAnterior DECIMAL(18,3);
    DECLARE @Diferenca DECIMAL(18,3);
    DECLARE @MovimentoInventario DECIMAL(18,3);
    DECLARE @EstoqueAtual DECIMAL(18,3);
    DECLARE @LinhasAfetadas INT;

    SELECT
        @OperacaoId = a.OperacaoId,
        @StatusId = a.StatusId,
        @ProdutoId = oi.ProdutoId,
        @QuantidadeAnterior = oi.Quantidade
    FROM dbo.OperacaoItem oi
    INNER JOIN dbo.Agendamento a ON a.Id = oi.AgendamentoId
    WHERE oi.Id = @Id
      AND oi.AgendamentoId = @AgendamentoId;

    IF @ProdutoId IS NULL
    BEGIN
        SELECT 0 AS LinhasAfetadas;
        RETURN;
    END

    IF @StatusId <> 3
    BEGIN
        THROW 50007, 'Itens so podem ser alterados quando o agendamento esta em doca.', 1;
    END

    SET @Diferenca = @Quantidade - @QuantidadeAnterior;
    SET @MovimentoInventario = CASE WHEN @OperacaoId = 1 THEN @Diferenca ELSE -@Diferenca END;

    BEGIN TRANSACTION;

        SELECT @EstoqueAtual = QuantidadeAtual
        FROM dbo.Inventario WITH (UPDLOCK, HOLDLOCK)
        WHERE ProdutoId = @ProdutoId;

        IF @EstoqueAtual IS NULL
        BEGIN
            THROW 50004, 'Inventario do produto nao encontrado.', 1;
        END

        IF @MovimentoInventario < 0 AND @EstoqueAtual < ABS(@MovimentoInventario)
        BEGIN
            THROW 50007, 'Estoque insuficiente para esta alteracao.', 1;
        END

        UPDATE dbo.Inventario
        SET
            QuantidadeAtual = QuantidadeAtual + @MovimentoInventario,
            DataUltimaAtualizacao = SYSDATETIME()
        WHERE ProdutoId = @ProdutoId;

        UPDATE dbo.OperacaoItem
        SET
            Quantidade = @Quantidade,
            AtualizadoEm = SYSDATETIME()
        WHERE Id = @Id
          AND AgendamentoId = @AgendamentoId;

        SET @LinhasAfetadas = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @LinhasAfetadas AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_OperacaoItem_Excluir
    @AgendamentoId INT,
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @OperacaoId INT;
    DECLARE @StatusId INT;
    DECLARE @ProdutoId INT;
    DECLARE @Quantidade DECIMAL(18,3);
    DECLARE @MovimentoInventario DECIMAL(18,3);
    DECLARE @EstoqueAtual DECIMAL(18,3);
    DECLARE @LinhasAfetadas INT;

    SELECT
        @OperacaoId = a.OperacaoId,
        @StatusId = a.StatusId,
        @ProdutoId = oi.ProdutoId,
        @Quantidade = oi.Quantidade
    FROM dbo.OperacaoItem oi
    INNER JOIN dbo.Agendamento a ON a.Id = oi.AgendamentoId
    WHERE oi.Id = @Id
      AND oi.AgendamentoId = @AgendamentoId;

    IF @ProdutoId IS NULL
    BEGIN
        SELECT 0 AS LinhasAfetadas;
        RETURN;
    END

    IF @StatusId <> 3
    BEGIN
        THROW 50007, 'Itens so podem ser alterados quando o agendamento esta em doca.', 1;
    END

    SET @MovimentoInventario = CASE WHEN @OperacaoId = 1 THEN -@Quantidade ELSE @Quantidade END;

    BEGIN TRANSACTION;

        SELECT @EstoqueAtual = QuantidadeAtual
        FROM dbo.Inventario WITH (UPDLOCK, HOLDLOCK)
        WHERE ProdutoId = @ProdutoId;

        IF @EstoqueAtual IS NULL
        BEGIN
            THROW 50004, 'Inventario do produto nao encontrado.', 1;
        END

        IF @MovimentoInventario < 0 AND @EstoqueAtual < ABS(@MovimentoInventario)
        BEGIN
            THROW 50007, 'Estoque insuficiente para remover este item.', 1;
        END

        UPDATE dbo.Inventario
        SET
            QuantidadeAtual = QuantidadeAtual + @MovimentoInventario,
            DataUltimaAtualizacao = SYSDATETIME()
        WHERE ProdutoId = @ProdutoId;

        DELETE FROM dbo.OperacaoItem
        WHERE Id = @Id
          AND AgendamentoId = @AgendamentoId;

        SET @LinhasAfetadas = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @LinhasAfetadas AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Veiculo_Listar
    @Placa VARCHAR(10) = NULL,
    @TipoVeiculo NVARCHAR(80) = NULL,
    @TransportadoraId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.Id,
        v.Placa,
        v.TipoVeiculo,
        v.TransportadoraId,
        t.Nome AS TransportadoraNome,
        v.Ativo,
        v.CriadoEm,
        v.AtualizadoEm
    FROM dbo.Veiculo v
    INNER JOIN dbo.Transportadora t ON t.Id = v.TransportadoraId
    WHERE v.Ativo = 1
      AND t.Ativo = 1
      AND (@Placa IS NULL OR v.Placa LIKE '%' + @Placa + '%')
      AND (@TipoVeiculo IS NULL OR v.TipoVeiculo LIKE '%' + @TipoVeiculo + '%')
      AND (@TransportadoraId IS NULL OR v.TransportadoraId = @TransportadoraId)
    ORDER BY v.Placa;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Veiculo_ObterPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.Id,
        v.Placa,
        v.TipoVeiculo,
        v.TransportadoraId,
        t.Nome AS TransportadoraNome,
        v.Ativo,
        v.CriadoEm,
        v.AtualizadoEm
    FROM dbo.Veiculo v
    INNER JOIN dbo.Transportadora t ON t.Id = v.TransportadoraId
    WHERE v.Id = @Id
      AND v.Ativo = 1
      AND t.Ativo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Relatorio_AgendamentosGeral
    @DataInicio DATE,
    @DataFim DATE,
    @OperacaoId INT = NULL,
    @StatusId INT = NULL,
    @Produto NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.Id AS Agenda,
        o.Descricao AS Operacao,
        s.Descricao AS Status,
        a.DataHoraAgendada AS DataAgendada,
        a.DataHoraChegada AS DataChegada,
        a.DataHoraDoca AS DataDoca,
        a.DataHoraFinalizado AS DataFinalizacao,
        t.Nome AS Transportadora,
        m.Nome AS Motorista,
        m.Cnh AS CNH,
        v.Placa,
        l.Descricao AS Doca
    FROM dbo.Agendamento a
    INNER JOIN dbo.Operacao o ON o.Id = a.OperacaoId
    INNER JOIN dbo.Status s ON s.Id = a.StatusId
    INNER JOIN dbo.Transportadora t ON t.Id = a.TransportadoraId
    INNER JOIN dbo.Motorista m ON m.Id = a.MotoristaId
    INNER JOIN dbo.Veiculo v ON v.Id = a.VeiculoId
    LEFT JOIN dbo.Local l ON l.Id = a.LocalId
    WHERE a.DataHoraAgendada >= @DataInicio
      AND a.DataHoraAgendada < DATEADD(DAY, 1, @DataFim)
      AND (@OperacaoId IS NULL OR a.OperacaoId = @OperacaoId)
      AND (@StatusId IS NULL OR a.StatusId = @StatusId)
    ORDER BY a.DataHoraAgendada, a.Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Relatorio_EstoqueAtual
    @DataInicio DATE,
    @DataFim DATE,
    @OperacaoId INT = NULL,
    @StatusId INT = NULL,
    @Produto NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Descricao AS Produto,
        i.QuantidadeAtual AS QuantidadeAtual,
        i.DataUltimaAtualizacao AS UltimaAtualizacao
    FROM dbo.Inventario i
    INNER JOIN dbo.Produto p ON p.Id = i.ProdutoId
    WHERE p.Ativo = 1
      AND (@Produto IS NULL OR p.Descricao LIKE '%' + @Produto + '%')
    ORDER BY p.Descricao;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Relatorio_AgendasFinalizadas
    @DataInicio DATE,
    @DataFim DATE,
    @OperacaoId INT = NULL,
    @StatusId INT = NULL,
    @Produto NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.Id AS Agenda,
        o.Descricao AS Operacao,
        t.Nome AS Transportadora,
        m.Nome AS Motorista,
        m.Cnh AS CNH,
        v.Placa,
        l.Descricao AS Doca,
        a.DataHoraAgendada AS DataAgendada,
        a.DataHoraChegada AS DataChegada,
        a.DataHoraDoca AS DataDoca,
        a.DataHoraFinalizado AS DataFinalizacao,
        DATEDIFF(MINUTE, a.DataHoraChegada, a.DataHoraFinalizado) AS TempoTotalMinutos,
        DATEDIFF(MINUTE, a.DataHoraDoca, a.DataHoraFinalizado) AS TempoEmDocaMinutos
    FROM dbo.Agendamento a
    INNER JOIN dbo.Operacao o ON o.Id = a.OperacaoId
    INNER JOIN dbo.Transportadora t ON t.Id = a.TransportadoraId
    INNER JOIN dbo.Motorista m ON m.Id = a.MotoristaId
    INNER JOIN dbo.Veiculo v ON v.Id = a.VeiculoId
    LEFT JOIN dbo.Local l ON l.Id = a.LocalId
    WHERE a.StatusId = 4
      AND a.DataHoraFinalizado >= @DataInicio
      AND a.DataHoraFinalizado < DATEADD(DAY, 1, @DataFim)
      AND (@OperacaoId IS NULL OR a.OperacaoId = @OperacaoId)
    ORDER BY a.DataHoraFinalizado, a.Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Relatorio_MovimentacaoEstoque
    @DataInicio DATE,
    @DataFim DATE,
    @OperacaoId INT = NULL,
    @StatusId INT = NULL,
    @Produto NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Descricao AS Produto,
        SUM(CASE WHEN a.OperacaoId = 1 THEN oi.Quantidade ELSE 0 END) AS QuantidadeRecebida,
        SUM(CASE WHEN a.OperacaoId = 2 THEN oi.Quantidade ELSE 0 END) AS QuantidadeEnviada,
        SUM(CASE WHEN a.OperacaoId = 1 THEN oi.Quantidade ELSE -oi.Quantidade END) AS SaldoMovimentado
    FROM dbo.OperacaoItem oi
    INNER JOIN dbo.Agendamento a ON a.Id = oi.AgendamentoId
    INNER JOIN dbo.Produto p ON p.Id = oi.ProdutoId
    WHERE a.StatusId = 4
      AND a.DataHoraFinalizado >= @DataInicio
      AND a.DataHoraFinalizado < DATEADD(DAY, 1, @DataFim)
      AND (@OperacaoId IS NULL OR a.OperacaoId = @OperacaoId)
      AND (@Produto IS NULL OR p.Descricao LIKE '%' + @Produto + '%')
    GROUP BY p.Descricao
    ORDER BY p.Descricao;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Relatorio_CargasRecebidasEnviadas
    @DataInicio DATE,
    @DataFim DATE,
    @OperacaoId INT = NULL,
    @StatusId INT = NULL,
    @Produto NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CAST(a.DataHoraFinalizado AS DATE) AS Data,
        SUM(CASE WHEN a.OperacaoId = 1 THEN 1 ELSE 0 END) AS CargasRecebidas,
        SUM(CASE WHEN a.OperacaoId = 2 THEN 1 ELSE 0 END) AS CargasEnviadas,
        COUNT(*) AS TotalCargas
    FROM dbo.Agendamento a
    WHERE a.StatusId = 4
      AND a.DataHoraFinalizado >= @DataInicio
      AND a.DataHoraFinalizado < DATEADD(DAY, 1, @DataFim)
      AND (@OperacaoId IS NULL OR a.OperacaoId = @OperacaoId)
    GROUP BY CAST(a.DataHoraFinalizado AS DATE)
    ORDER BY Data;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Relatorio_PerformanceOperacional
    @DataInicio DATE,
    @DataFim DATE,
    @OperacaoId INT = NULL,
    @StatusId INT = NULL,
    @Produto NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.Id AS Agenda,
        o.Descricao AS Operacao,
        t.Nome AS Transportadora,
        l.Descricao AS Doca,
        a.DataHoraAgendada AS DataAgendada,
        a.DataHoraChegada AS DataChegada,
        a.DataHoraDoca AS DataDoca,
        a.DataHoraFinalizado AS DataFinalizacao,
        DATEDIFF(MINUTE, a.DataHoraAgendada, a.DataHoraChegada) AS AtrasoOuAntecedenciaCheckInMinutos,
        DATEDIFF(MINUTE, a.DataHoraChegada, a.DataHoraDoca) AS EsperaParaDocaMinutos,
        DATEDIFF(MINUTE, a.DataHoraDoca, a.DataHoraFinalizado) AS TempoEmDocaMinutos,
        DATEDIFF(MINUTE, a.DataHoraChegada, a.DataHoraFinalizado) AS TempoTotalOperacaoMinutos
    FROM dbo.Agendamento a
    INNER JOIN dbo.Operacao o ON o.Id = a.OperacaoId
    INNER JOIN dbo.Transportadora t ON t.Id = a.TransportadoraId
    LEFT JOIN dbo.Local l ON l.Id = a.LocalId
    WHERE a.StatusId = 4
      AND a.DataHoraFinalizado >= @DataInicio
      AND a.DataHoraFinalizado < DATEADD(DAY, 1, @DataFim)
      AND (@OperacaoId IS NULL OR a.OperacaoId = @OperacaoId)
    ORDER BY a.DataHoraFinalizado, a.Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Veiculo_Inserir
    @Placa VARCHAR(10),
    @TipoVeiculo NVARCHAR(80),
    @TransportadoraId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Transportadora WHERE Id = @TransportadoraId AND Ativo = 1)
    BEGIN
        THROW 50002, 'Transportadora nao encontrada ou inativa.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.Veiculo WHERE Placa = @Placa AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe um veiculo cadastrado com esta placa.', 1;
    END

    INSERT INTO dbo.Veiculo (Placa, TipoVeiculo, TransportadoraId)
    VALUES (@Placa, @TipoVeiculo, @TransportadoraId);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Veiculo_Atualizar
    @Id INT,
    @Placa VARCHAR(10),
    @TipoVeiculo NVARCHAR(80),
    @TransportadoraId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Transportadora WHERE Id = @TransportadoraId AND Ativo = 1)
    BEGIN
        THROW 50002, 'Transportadora nao encontrada ou inativa.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.Veiculo WHERE Placa = @Placa AND Id <> @Id AND Ativo = 1)
    BEGIN
        THROW 50001, 'Ja existe um veiculo cadastrado com esta placa.', 1;
    END

    UPDATE dbo.Veiculo
    SET
        Placa = @Placa,
        TipoVeiculo = @TipoVeiculo,
        TransportadoraId = @TransportadoraId,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Veiculo_Excluir
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Veiculo
    SET
        Ativo = 0,
        AtualizadoEm = SYSDATETIME()
    WHERE Id = @Id
      AND Ativo = 1;

    SELECT @@ROWCOUNT AS LinhasAfetadas;
END
GO
