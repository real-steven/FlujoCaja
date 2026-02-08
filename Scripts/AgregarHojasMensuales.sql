-- ============================================
-- SCRIPT DE MIGRACIÓN - HOJAS MENSUALES
-- Sistema de Flujo de Caja - Supabase
-- Fecha: 8 de Enero 2026
-- ============================================

-- ============================================
-- 1. CREAR SECUENCIA PARA HOJAS MENSUALES
-- ============================================

CREATE SEQUENCE IF NOT EXISTS public.hojas_mensuales_id_seq;

-- ============================================
-- 2. CREAR TABLA HOJAS_MENSUALES
-- ============================================

CREATE TABLE IF NOT EXISTS public.hojas_mensuales (
  id integer NOT NULL DEFAULT nextval('hojas_mensuales_id_seq'::regclass),
  casaid integer NOT NULL,
  mes integer NOT NULL CHECK (mes >= 1 AND mes <= 12),
  anio integer NOT NULL CHECK (anio >= 2020),
  fechacreacion timestamp with time zone DEFAULT now(),
  cerrada boolean DEFAULT false,
  CONSTRAINT hojas_mensuales_pkey PRIMARY KEY (id),
  CONSTRAINT hojas_mensuales_casaid_fkey FOREIGN KEY (casaid) REFERENCES public.casas(id) ON DELETE CASCADE,
  CONSTRAINT hojas_mensuales_unique UNIQUE (casaid, mes, anio)
);

-- ============================================
-- 3. AGREGAR COLUMNA hoja_mensual_id A MOVIMIENTOS
-- ============================================

-- Verificar si la columna ya existe antes de agregarla
DO $$ 
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.columns 
    WHERE table_schema = 'public' 
    AND table_name = 'movimientos' 
    AND column_name = 'hoja_mensual_id'
  ) THEN
    ALTER TABLE public.movimientos 
    ADD COLUMN hoja_mensual_id integer;
    
    ALTER TABLE public.movimientos
    ADD CONSTRAINT movimientos_hoja_mensual_fkey 
    FOREIGN KEY (hoja_mensual_id) 
    REFERENCES public.hojas_mensuales(id) 
    ON DELETE SET NULL;
  END IF;
END $$;

-- ============================================
-- 4. CREAR ÍNDICES
-- ============================================

CREATE INDEX IF NOT EXISTS idx_hojas_mensuales_casa ON public.hojas_mensuales(casaid);
CREATE INDEX IF NOT EXISTS idx_hojas_mensuales_anio_mes ON public.hojas_mensuales(anio, mes);
CREATE INDEX IF NOT EXISTS idx_movimientos_hoja_mensual ON public.movimientos(hoja_mensual_id);

-- ============================================
-- 5. INSERTAR HOJAS MENSUALES DE 2025 (12 meses)
-- ============================================

-- Para cada casa existente, crear las 12 hojas de 2025
DO $$ 
DECLARE
  casa_record RECORD;
  mes_num integer;
BEGIN
  -- Recorrer todas las casas
  FOR casa_record IN SELECT id FROM public.casas LOOP
    -- Crear hojas de enero a diciembre 2025
    FOR mes_num IN 1..12 LOOP
      INSERT INTO public.hojas_mensuales (casaid, mes, anio, cerrada)
      VALUES (casa_record.id, mes_num, 2025, false)
      ON CONFLICT (casaid, mes, anio) DO NOTHING;
    END LOOP;
    
    -- Crear hojas de enero y febrero 2026
    INSERT INTO public.hojas_mensuales (casaid, mes, anio, cerrada)
    VALUES (casa_record.id, 1, 2026, false)
    ON CONFLICT (casaid, mes, anio) DO NOTHING;
    
    INSERT INTO public.hojas_mensuales (casaid, mes, anio, cerrada)
    VALUES (casa_record.id, 2, 2026, false)
    ON CONFLICT (casaid, mes, anio) DO NOTHING;
  END LOOP;
END $$;

-- ============================================
-- 6. ASIGNAR MOVIMIENTOS EXISTENTES A HOJA ENERO 2026
-- ============================================

-- Todos los movimientos existentes se asignan a la hoja de Enero 2026
-- (Asume que son movimientos de prueba del mes actual)
DO $$
DECLARE
  movimiento_record RECORD;
  hoja_id integer;
BEGIN
  FOR movimiento_record IN SELECT id, casaid FROM public.movimientos WHERE hoja_mensual_id IS NULL LOOP
    -- Buscar la hoja de Enero 2026 para esta casa
    SELECT id INTO hoja_id
    FROM public.hojas_mensuales
    WHERE casaid = movimiento_record.casaid
    AND mes = 1
    AND anio = 2026
    LIMIT 1;
    
    -- Asignar el movimiento a la hoja
    IF hoja_id IS NOT NULL THEN
      UPDATE public.movimientos
      SET hoja_mensual_id = hoja_id
      WHERE id = movimiento_record.id;
    END IF;
  END LOOP;
END $$;

-- ============================================
-- 7. HABILITAR ROW LEVEL SECURITY EN HOJAS_MENSUALES
-- ============================================

ALTER TABLE public.hojas_mensuales ENABLE ROW LEVEL SECURITY;

DROP POLICY IF EXISTS "Acceso completo hojas_mensuales" ON public.hojas_mensuales;
CREATE POLICY "Acceso completo hojas_mensuales" 
ON public.hojas_mensuales 
FOR ALL 
USING (auth.role() = 'authenticated');

-- ============================================
-- 8. HABILITAR REALTIME EN HOJAS_MENSUALES
-- ============================================

ALTER PUBLICATION supabase_realtime ADD TABLE public.hojas_mensuales;

-- ============================================
-- 9. VERIFICACIÓN
-- ============================================

SELECT 'Hojas mensuales creadas:' as tabla, COUNT(*) as total FROM public.hojas_mensuales;

-- Ver resumen de hojas por casa
SELECT 
  c.nombre as casa,
  hm.anio,
  COUNT(*) as meses_creados
FROM public.hojas_mensuales hm
JOIN public.casas c ON hm.casaid = c.id
GROUP BY c.nombre, hm.anio
ORDER BY c.nombre, hm.anio;

-- Ver movimientos asignados a hojas
SELECT 
  c.nombre as casa,
  hm.mes,
  hm.anio,
  COUNT(m.id) as movimientos
FROM public.hojas_mensuales hm
JOIN public.casas c ON hm.casaid = c.id
LEFT JOIN public.movimientos m ON m.hoja_mensual_id = hm.id
GROUP BY c.nombre, hm.mes, hm.anio
ORDER BY c.nombre, hm.anio, hm.mes;

-- ============================================
-- FIN DEL SCRIPT DE MIGRACIÓN
-- ============================================

-- ═══════════════════════════════════════════
-- NOTAS IMPORTANTES
-- ═══════════════════════════════════════════
-- 
-- 1. HOJAS MENSUALES:
--    - Se crean automáticamente para cada casa
--    - Constraint UNIQUE evita duplicados (casaid + mes + anio)
--    - Relación CASCADE: si se elimina casa, se eliminan sus hojas
--
-- 2. MOVIMIENTOS:
--    - Ahora tienen columna hoja_mensual_id
--    - Los movimientos existentes se asignan a Enero 2026
--    - SET NULL: si se elimina hoja, movimiento queda sin asignar
--
-- 3. CREACIÓN DE HOJAS:
--    - 2025: 12 meses completos (Enero a Diciembre)
--    - 2026: Solo Enero y Febrero (mes actual + siguiente)
--
-- 4. LÓGICA DE ASIGNACIÓN:
--    - Los movimientos NO se asignan por su fecha
--    - Se asignan a la hoja seleccionada al momento de crearlos
--    - Ejemplo: Movimiento con fecha 15/12/2025 puede estar en hoja Enero 2026
