USE GoCarDB;
GO

/* =========================================================
   1. CATEGORIAS
   ========================================================= */

INSERT INTO dbo.Categorias
    (Nome, Descricao, DiariaBase, KmLivre, IsAtivo)
VALUES
    ('Econômico', 'Veículos econômicos e compactos', 110.00, 0, 1),
    ('Hatch',     'Veículos hatch compactos',        135.00, 0, 1),
    ('Sedan',     'Veículos sedan',                  170.00, 0, 1),
    ('SUV',       'Veículos utilitários esportivos', 200.00, 0, 1);


/* =========================================================
   2. FILIAIS
   Dados fictícios para desenvolvimento
   ========================================================= */

INSERT INTO dbo.Filiais
(
    Nome,
    CNPJ,
    Telefone,
    Email,
    Endereco,
    Numero,
    Bairro,
    Cidade,
    Estado,
    CEP,
    IsAtivo
)
VALUES
(
    'GoCar Guarulhos',
    '00.000.000/0001-01',
    '(11) 4000-1001',
    'guarulhos@gocar.com',
    'Avenida GoCar',
    '100',
    'Centro',
    'Guarulhos',
    'SP',
    '07000-000',
    1
),
(
    'GoCar São Paulo Centro',
    '00.000.000/0002-01',
    '(11) 4000-1002',
    'centro@gocar.com',
    'Avenida GoCar',
    '200',
    'Centro',
    'São Paulo',
    'SP',
    '01000-000',
    1
),
(
    'GoCar Tatuapé Premium',
    '00.000.000/0003-01',
    '(11) 4000-1003',
    'tatuape@gocar.com',
    'Avenida GoCar',
    '300',
    'Tatuapé',
    'São Paulo',
    'SP',
    '03000-000',
    1
);


/* =========================================================
   3. MODELOS DE VEÍCULOS
   ========================================================= */

DECLARE @Modelos TABLE
(
    ModeloId INT IDENTITY(1,1),
    Categoria VARCHAR(50),
    Marca VARCHAR(50),
    Modelo VARCHAR(100),
    AnoFabricacao SMALLINT,
    AnoModelo SMALLINT,
    ValorDiaria DECIMAL(10,2),
    Cambio INT
);

INSERT INTO @Modelos
(
    Categoria,
    Marca,
    Modelo,
    AnoFabricacao,
    AnoModelo,
    ValorDiaria,
    Cambio
)
VALUES

/* ECONÔMICO */
('Econômico','Fiat','Mobi 1.0',2024,2025,120.00,1),
('Econômico','Renault','Kwid 1.0',2024,2025,120.00,1),
('Econômico','Volkswagen','Polo Track 1.0',2024,2025,135.00,1),
('Econômico','Fiat','Uno Attractive 1.0',2021,2021,110.00,1),
('Econômico','Ford','Ka 1.0',2021,2021,115.00,1),
('Econômico','Hyundai','HB20 Sense 1.0 (Entrada)',2023,2024,125.00,1),

/* HATCH */
('Hatch','Chevrolet','Onix 1.0',2024,2025,150.00,1),
('Hatch','Hyundai','HB20 1.0',2024,2025,150.00,1),
('Hatch','Fiat','Argo 1.0',2024,2025,145.00,1),
('Hatch','Peugeot','208 Like 1.0',2024,2025,140.00,1),
('Hatch','Citroën','C3 Live 1.0',2024,2025,135.00,1),
('Hatch','Volkswagen','Polo MPI 1.0',2024,2025,155.00,1),

/* SEDAN */
('Sedan','Chevrolet','Onix Plus 1.0 Turbo',2024,2025,180.00,2),
('Sedan','Hyundai','HB20S 1.0',2024,2025,180.00,2),
('Sedan','Toyota','Corolla 2.0',2023,2024,320.00,2),
('Sedan','Fiat','Cronos 1.3',2024,2025,170.00,2),
('Sedan','Volkswagen','Virtus 1.0 TSI',2024,2025,195.00,2),
('Sedan','Nissan','Versa 1.6',2024,2025,190.00,4),

/* SUV */
('SUV','Fiat','Pulse 1.3',2024,2025,200.00,2),
('SUV','Jeep','Renegade 1.3 Turbo',2024,2025,240.00,2),
('SUV','Hyundai','Creta 1.0 Turbo',2024,2025,250.00,2),
('SUV','Volkswagen','T-Cross 1.0 TSI',2024,2025,260.00,2),
('SUV','Chevrolet','Tracker 1.0 Turbo',2024,2025,250.00,2),
('SUV','Nissan','Kicks 1.6',2024,2025,230.00,4);


/* =========================================================
   4. CRIAR 10 UNIDADES DE CADA MODELO
   ========================================================= */

DECLARE @Numero INT = 1;

WHILE @Numero <= 10
BEGIN

    INSERT INTO dbo.Veiculos
    (
        Placa,
        Chassi,
        Renavam,
        Modelo,
        Marca,
        AnoFabricacao,
        AnoModelo,
        Cor,
        Combustivel,
        Cambio,
        Status,
        KmAtual,
        ValorDiaria,
        IsAtivo,
        CategoriaId,
        FilialId
    )

    SELECT

        /* Placa fictícia única */
        'GC' +
        RIGHT('00000' + CAST(
            ((M.ModeloId - 1) * 10) + @Numero
            AS VARCHAR(5)),5),

        /* Chassi fictício único */
        'GOCAR-CHASSI-' +
        RIGHT('000000' + CAST(
            ((M.ModeloId - 1) * 10) + @Numero
            AS VARCHAR(6)),6),

        /* Renavam fictício único */
        '900000' +
        RIGHT('00000' + CAST(
            ((M.ModeloId - 1) * 10) + @Numero
            AS VARCHAR(5)),5),

        M.Modelo,
        M.Marca,
        M.AnoFabricacao,
        M.AnoModelo,

        /* Cores variadas */
        CASE (@Numero % 5)
            WHEN 0 THEN 'Preto'
            WHEN 1 THEN 'Branco'
            WHEN 2 THEN 'Prata'
            WHEN 3 THEN 'Cinza'
            ELSE 'Vermelho'
        END,

        /* TipoCombustivel.Flex = 3 */
        3,

        M.Cambio,

        /* StatusVeiculo.Disponivel = 1 */
        1,

        /* Quilometragem inicial fictícia */
        (@Numero - 1) * 1250,

        M.ValorDiaria,

        1,

        C.Id,

        /* Distribui os veículos entre as 3 filiais */
        CASE
            WHEN @Numero IN (1,2,3,4)
                THEN (SELECT Id FROM dbo.Filiais
                      WHERE Nome = 'GoCar Guarulhos')

            WHEN @Numero IN (5,6,7)
                THEN (SELECT Id FROM dbo.Filiais
                      WHERE Nome = 'GoCar São Paulo Centro')

            ELSE
                (SELECT Id FROM dbo.Filiais
                 WHERE Nome = 'GoCar Tatuapé Premium')
        END

    FROM @Modelos M

    INNER JOIN dbo.Categorias C
        ON C.Nome = M.Categoria;

    SET @Numero = @Numero + 1;

END;


/* =========================================================
   5. CONFERÊNCIA
   ========================================================= */

SELECT COUNT(*) AS TotalCategorias
FROM dbo.Categorias;

SELECT COUNT(*) AS TotalFiliais
FROM dbo.Filiais;

SELECT COUNT(*) AS TotalVeiculos
FROM dbo.Veiculos;


/* Quantidade por categoria */

SELECT
    C.Nome AS Categoria,
    COUNT(*) AS Quantidade
FROM dbo.Veiculos V
INNER JOIN dbo.Categorias C
    ON V.CategoriaId = C.Id
GROUP BY C.Nome
ORDER BY C.Nome;


/* Quantidade por modelo */

SELECT
    Marca,
    Modelo,
    COUNT(*) AS Quantidade
FROM dbo.Veiculos
GROUP BY Marca, Modelo
ORDER BY Marca, Modelo;


/* Quantidade por filial */

SELECT
    F.Nome AS Filial,
    COUNT(*) AS Quantidade
FROM dbo.Veiculos V
INNER JOIN dbo.Filiais F
    ON V.FilialId = F.Id
GROUP BY F.Nome
ORDER BY F.Nome;