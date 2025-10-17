-- Script para agregar campo de moneda a la tabla casas
-- Ejecutar en Supabase SQL Editor

-- 1. Agregar la columna moneda a la tabla casas
ALTER TABLE casas 
ADD COLUMN IF NOT EXISTS moneda VARCHAR(3) DEFAULT 'USD';

-- 2. Agregar comentario a la columna
COMMENT ON COLUMN casas.moneda IS 'Código de moneda utilizada para la casa (USD, CRC, EUR)';

-- 3. Crear un check constraint para validar los valores de moneda
ALTER TABLE casas 
ADD CONSTRAINT check_moneda_valida 
CHECK (moneda IN ('USD', 'CRC', 'EUR'));

-- 4. Actualizar registros existentes que no tengan moneda (si los hay)
UPDATE casas 
SET moneda = 'USD' 
WHERE moneda IS NULL;

-- 5. Hacer la columna NOT NULL después de actualizar registros existentes
ALTER TABLE casas 
ALTER COLUMN moneda SET NOT NULL;

-- 6. Crear índice en la columna moneda para mejorar consultas
CREATE INDEX IF NOT EXISTS idx_casas_moneda ON casas(moneda);

-- Verificar que todo se aplicó correctamente
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns 
WHERE table_name = 'casas' 
AND column_name = 'moneda';

-- Verificar el constraint
SELECT constraint_name, check_clause
FROM information_schema.check_constraints 
WHERE constraint_name = 'check_moneda_valida';

-- Ver estructura actualizada de la tabla
\d casas;