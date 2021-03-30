CREATE SCHEMA EQL

	CREATE TABLE [EQL].[Lookups] (
		[Id] INT IDENTITY,
		[TermKey] VARCHAR(256) NOT NULL,
		[Code] INT NOT NULL,
		[Value] NVARCHAR(MAX) NOT NULL,
		[DisplayOrder] INT NOT NULL
		PRIMARY KEY (Id)
	)

	CREATE VIEW [EQL].[v_TermKeys] AS 
		SELECT
				[TermKey] = EP.value
			,[TableObjectId] = EP.major_id
			,[ColumnObjectId] = EP.minor_id
		FROM sys.extended_properties EP
		WHERE EP.name = 'TermKey'

	CREATE VIEW [EQL].[v_EntityKeys] AS 
		SELECT
			[EntityKey] = EP.value,
			[TableObjectId] = EP.major_id
		FROM sys.extended_properties EP
		WHERE EP.name = 'EntityKey'

	CREATE VIEW [EQL].[v_PrimaryKeys] AS 
		SELECT 
			[PrimaryKeyName] = KC.name,
			[TableObjectId] = KC.parent_object_id,
			[ColumnId] = SC.column_id,
			[ColumnName] = QUOTENAME(KCU.COLUMN_NAME)
		FROM sys.key_constraints KC
			INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU ON KCU.CONSTRAINT_NAME = KC.name
			LEFT JOIN sys.columns SC ON SC.object_id = KC.parent_object_id AND SC.name = KCU.COLUMN_NAME 
		WHERE KC.[type] = 'PK'

	CREATE VIEW [EQL].[v_SystemColumns] AS 
		SELECT
			 [ObjectId] = SC.object_id
			,[ColumnId] = SC.column_id
			,[ColumnName] = QUOTENAME(SC.name)
			,[DataType] = DT.name
			,[IsIdentity] = SC.is_identity
			,[IsNullable] = SC.is_nullable
			,[MaxLength] = SC.max_length
			,[Precision] = SC.precision
			,[Scale] = SC.scale
		FROM sys.columns SC
			LEFT JOIN sys.types DT ON DT.user_type_id = SC.user_type_id

	CREATE VIEW [EQL].[v_EntityFields] AS
		SELECT
			 [ObjectId] = SC.ObjectId
			,[ColumnName] = SC.ColumnName
			,[TermKey] = TK.TermKey
			,[DataType] = SC.DataType
			,[IsIdentity] = SC.IsIdentity
			,[IsPrimaryKey] = CONVERT(BIT, iif(PK.ColumnName = SC.ColumnName, 1, 0))
			,[IsNullable] = SC.IsNullable
			,[MaxLength] = SC.MaxLength
			,[Precision] = SC.Precision
			,[Scale] = SC.scale
			,[IsLookup] = 
				CASE
					WHEN EXISTS (SELECT TOP 1 1 FROM EQL.Lookups L WHERE L.TermKey = TK.TermKey) THEN CONVERT(BIT, 1)
					ELSE CONVERT(BIT, 0)
				END
		FROM EQL.v_SystemColumns SC
			LEFT JOIN EQL.v_TermKeys TK ON TK.TableObjectId = SC.ObjectId AND TK.ColumnObjectId = SC.ColumnId
			LEFT JOIN EQL.v_PrimaryKeys PK ON PK.TableObjectId = SC.ObjectId AND PK.ColumnId = SC.ColumnId
		WHERE TK.TermKey IS NOT NULL

	CREATE VIEW [EQL].[v_EntityTypes] AS
		WITH Inserts AS (
			SELECT [TableObjectId] = major_id FROM sys.extended_properties WHERE Name = 'CanInsert'
		)

		, Updates AS (
			SELECT [TableObjectId] = major_id FROM sys.extended_properties WHERE Name = 'CanUpdate'
		)
	
		, Deletes AS (
			SELECT [TableObjectId] = major_id FROM sys.extended_properties WHERE Name = 'CanDelete'
		)

		, EligibleEntities AS (
			SELECT
				 [ObjectId] = SV.object_id
				,[SchemaId] = SV.schema_id
				,[Name] = SV.name
			FROM sys.views SV

			UNION

			SELECT
				 [ObjectId] = ST.object_id
				,[SchemaId] = ST.schema_id
				,[Name] = ST.name
			FROM sys.tables ST
		)

		SELECT
			 [Name] =  QUOTENAME(SS.name) + '.' + QUOTENAME(EE.name)
			,[SchemaId] = SS.schema_id
			,[ObjectId] = EE.ObjectId
			,[EntityKey] = EK.EntityKey
			,[CanInsert] = CONVERT(BIT, iif(I.TableObjectId IS NOT NULL, 1, 0))
			,[CanUpdate] = CONVERT(BIT, iif(U.TableObjectId IS NOT NULL, 1, 0))
			,[CanDelete] = CONVERT(BIT, iif(D.TableObjectId IS NOT NULL, 1, 0))
		FROM EQL.v_EntityKeys EK
			LEFT JOIN EligibleEntities EE ON EE.ObjectId = EK.TableObjectId
			LEFT JOIN sys.schemas SS ON SS.schema_id = EE.SchemaId
			LEFT JOIN Inserts I ON I.TableObjectId = EE.ObjectId
			LEFT JOIN Updates U ON U.TableObjectId = EE.ObjectId
			LEFT JOIN Deletes D ON D.TableObjectId = EE.ObjectId
GO

CREATE PROCEDURE [EQL].[AddEntityKey] (
	 @EntityKey SQL_VARIANT
	,@Schema sysname
	,@Table sysname
	) AS
	BEGIN
		EXEC sp_addextendedproperty 
			 @name = N'EntityKey' 
			,@value = @EntityKey
			,@level0type = N'Schema', @level0name = @Schema 
			,@level1type = N'Table',  @level1name = @Table
	END
GO

CREATE PROCEDURE [EQL].[AddTermKey] (
	 @TermKey SQL_VARIANT
	,@Schema sysname
	,@Table sysname
	,@Column sysname
	) AS
	BEGIN
		EXEC sp_addextendedproperty 
			 @name = N'TermKey' 
			,@value = @TermKey 
			,@level0type = N'Schema', @level0name = @Schema 
			,@level1type = N'Table',  @level1name = @Table 
			,@level2type = N'Column', @level2name = @Column
	END
GO

CREATE PROCEDURE [EQL].[CanInsertEntity] (
	 @Schema sysname
	,@Table sysname
	) AS
	BEGIN
		
		EXEC sp_addextendedproperty 
			 @name = N'CanInsert' 
			,@value = 1
			,@level0type = N'Schema', @level0name = @Schema 
			,@level1type = N'Table',  @level1name = @Table
	END
GO

CREATE PROCEDURE [EQL].[CanUpdateEntity] (
	 @Schema sysname
	,@Table sysname
	) AS
	BEGIN
		
		EXEC sp_addextendedproperty 
			 @name = N'CanUpdate' 
			,@value = 1
			,@level0type = N'Schema', @level0name = @Schema 
			,@level1type = N'Table',  @level1name = @Table
	END
GO

CREATE PROCEDURE [EQL].[CanDeleteEntity] (
	 @Schema sysname
	,@Table sysname
	) AS
	BEGIN
		
		EXEC sp_addextendedproperty 
			 @name = N'CanDelete' 
			,@value = 1
			,@level0type = N'Schema', @level0name = @Schema 
			,@level1type = N'Table',  @level1name = @Table
	END
GO

CREATE PROCEDURE EQL.GetSchemaDefinition AS 
	BEGIN

		SELECT
				[EntityTypes] = JSON_QUERY(
				(SELECT
					 [DatabaseName] = ET.Name
					,[EntityKey]    = ET.EntityKey
					,[CanInsert]    = ET.CanInsert
					,[CanUpdate]    = ET.CanUpdate
					,[CanDelete]    = ET.CanDelete
					,[EntityFields] = JSON_QUERY(
						(SELECT
								[ColumnName] = EF.ColumnName
							,[TermKey] = EF.TermKey
							,[DataType] = EF.DataType
							,[IsIdentity] = EF.IsIdentity
							,[IsPrimaryKey] = EF.IsPrimaryKey
							,[IsNullable] = EF.IsNullable
							,[MaxLength] = EF.MaxLength
							,[Precision] = EF.Precision
							,[Scale] = EF.Scale
							,[IsLookup] = EF.IsLookup
						FROM EQL.v_EntityFields EF
						WHERE EF.ObjectId = ET.ObjectId AND EF.TermKey IS NOT NULL
						FOR JSON PATH)
					)
				FROM EQL.v_EntityTypes ET 
					LEFT JOIN sys.schemas SS ON SS.schema_id = ET.SchemaId
				FOR JSON PATH)
			)
			,[Lookups] = JSON_QUERY(
				(SELECT
					 [TermKey] = L.TermKey
					,[DisplayOrder] = L.DisplayOrder
					,[Code] = L.Code
					,[Value] = L.Value
				FROM EQL.Lookups L
				ORDER BY L.TermKey, L.DisplayOrder
				FOR JSON PATH)
			)
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
	END
GO