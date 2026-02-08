-- =====================================================
-- Script: Resetear secuencia de categorias_movimientos
-- Fecha: 2026-01-28
-- Descripción: Ajusta la secuencia al valor máximo actual
-- =====================================================

-- Resetear secuencia al valor correcto
DO $$
DECLARE
  max_id INTEGER;
BEGIN
  SELECT COALESCE(MAX(id), 0) INTO max_id FROM categorias_movimientos;
  IF max_id > 0 THEN
    PERFORM setval('categorias_movimientos_id_seq', max_id, true);
  ELSE
    PERFORM setval('categorias_movimientos_id_seq', 1, false);
  END IF;
  
  RAISE NOTICE 'Secuencia categorias_movimientos_id_seq ajustada. Max ID: %', max_id;
END $$;

-- Verificación
SELECT 
  'categorias_movimientos_id_seq' as secuencia,
  last_value as ultimo_valor,
  is_called
FROM categorias_movimientos_id_seq;

-- Ver categorías existentes
SELECT 
  id, 
  nombre, 
  tipo, 
  activo 
FROM categorias_movimientos 
ORDER BY id;
