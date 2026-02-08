-- =====================================================
-- Script de migración: Cambiar tipo_entidad a tipo
-- Fecha: 2026-01-10
-- Descripción: Convierte la columna tipo_entidad a tipo
--              con valores 'ingreso' o 'egreso'
-- =====================================================

-- 1. Agregar nueva columna 'tipo'
ALTER TABLE public.categorias_movimientos
ADD COLUMN tipo character varying;

-- 2. Migrar datos existentes (asumiendo que todas son 'ingreso' por defecto)
-- NOTA: Ajusta los nombres de categorías según tu base de datos real
UPDATE public.categorias_movimientos
SET tipo = 'ingreso'
WHERE nombre IN ('Salario', 'Bono', 'Inversiones', 'Intereses', 'Venta', 'Alquiler Recibido', 'Reembolso', 'Regalo Recibido', 'Otros Ingresos');

UPDATE public.categorias_movimientos
SET tipo = 'egreso'
WHERE tipo IS NULL;

-- 3. Hacer la columna NOT NULL con CHECK constraint
ALTER TABLE public.categorias_movimientos
ALTER COLUMN tipo SET NOT NULL;

ALTER TABLE public.categorias_movimientos
ADD CONSTRAINT categorias_movimientos_tipo_check CHECK (tipo IN ('ingreso', 'egreso'));

-- 4. Eliminar la columna tipo_entidad antigua
ALTER TABLE public.categorias_movimientos
DROP COLUMN tipo_entidad;

-- Verificación
SELECT id, nombre, tipo FROM public.categorias_movimientos ORDER BY tipo, nombre;
