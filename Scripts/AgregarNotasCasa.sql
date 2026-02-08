-- =====================================================
-- Script: Agregar tabla notas_casa
-- Fecha: 2026-01-10
-- Descripción: Tabla para almacenar notas/observaciones
--              asociadas a cada casa
-- =====================================================

-- Crear tabla notas_casa
CREATE TABLE IF NOT EXISTS public.notas_casa (
  id SERIAL PRIMARY KEY,
  casaid integer NOT NULL,
  contenido text NOT NULL,
  fechacreacion timestamp with time zone NOT NULL DEFAULT now(),
  CONSTRAINT fk_notas_casa FOREIGN KEY (casaid) REFERENCES public.casas(id) ON DELETE CASCADE
);

-- Crear índice para búsquedas por casa
CREATE INDEX IF NOT EXISTS idx_notas_casa_casaid ON public.notas_casa(casaid);

-- Crear índice para ordenar por fecha
CREATE INDEX IF NOT EXISTS idx_notas_casa_fecha ON public.notas_casa(fechacreacion DESC);

-- Habilitar Row Level Security
ALTER TABLE public.notas_casa ENABLE ROW LEVEL SECURITY;

-- Policy para SELECT (todos los usuarios autenticados pueden leer)
CREATE POLICY "Usuarios autenticados pueden ver notas" ON public.notas_casa
  FOR SELECT
  USING (auth.role() = 'authenticated');

-- Policy para INSERT (todos los usuarios autenticados pueden insertar)
CREATE POLICY "Usuarios autenticados pueden agregar notas" ON public.notas_casa
  FOR INSERT
  WITH CHECK (auth.role() = 'authenticated');

-- Policy para UPDATE (todos los usuarios autenticados pueden actualizar)
CREATE POLICY "Usuarios autenticados pueden actualizar notas" ON public.notas_casa
  FOR UPDATE
  USING (auth.role() = 'authenticated');

-- Policy para DELETE (todos los usuarios autenticados pueden eliminar)
CREATE POLICY "Usuarios autenticados pueden eliminar notas" ON public.notas_casa
  FOR DELETE
  USING (auth.role() = 'authenticated');

-- Habilitar realtime
ALTER PUBLICATION supabase_realtime ADD TABLE public.notas_casa;

-- Comentarios
COMMENT ON TABLE public.notas_casa IS 'Notas y observaciones asociadas a cada casa';
COMMENT ON COLUMN public.notas_casa.id IS 'ID único de la nota';
COMMENT ON COLUMN public.notas_casa.casaid IS 'Referencia a la casa';
COMMENT ON COLUMN public.notas_casa.contenido IS 'Texto de la nota';
COMMENT ON COLUMN public.notas_casa.fechacreacion IS 'Fecha y hora de creación de la nota';

-- Resetear secuencia al valor correcto
DO $$
DECLARE
  max_id INTEGER;
BEGIN
  SELECT COALESCE(MAX(id), 0) INTO max_id FROM notas_casa;
  IF max_id > 0 THEN
    PERFORM setval('notas_casa_id_seq', max_id, true);
  ELSE
    PERFORM setval('notas_casa_id_seq', 1, false);
  END IF;
END $$;

-- Verificación
SELECT 'Tabla notas_casa creada exitosamente' AS mensaje;
