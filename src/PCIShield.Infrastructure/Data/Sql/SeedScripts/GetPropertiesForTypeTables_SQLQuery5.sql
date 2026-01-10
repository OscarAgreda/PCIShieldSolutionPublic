USE PCIShield_Core_Db
GO
SELECT
    t.name AS TableName,
    STRING_AGG(
        c.name + ' ' +
        ty.name +
        COALESCE('(' + 
            CASE 
                WHEN ty.name IN ('nvarchar','varchar','char','nchar') 
                THEN 
                    CASE 
                        WHEN c.max_length = -1 THEN 'MAX'
                        ELSE CAST(c.max_length AS VARCHAR(10))
                    END
                ELSE NULL
            END 
        + ')','')
        + CASE WHEN c.is_nullable = 1 THEN ' NULL' ELSE ' NOT NULL' END,
        ', '
    ) WITHIN GROUP (ORDER BY c.column_id) AS ColumnsDefinition
FROM sys.tables t
JOIN sys.columns c 
    ON t.object_id = c.object_id
JOIN sys.types ty 
    ON c.user_type_id = ty.user_type_id
WHERE t.name LIKE '%Type'
  AND t.is_ms_shipped = 0
  AND ty.is_user_defined = 0
GROUP BY t.name
ORDER BY t.name;