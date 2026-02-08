-- =====================================================
-- Script: Agregar tabla fotos_casa
-- Fecha: 2026-01-10
-- Descripción: Tabla para almacenar URLs de fotos
--              de las casas desde Supabase Storage
-- =====================================================

-- Crear secuencia
CREATE SEQUENCE IF NOT EXISTS public.fotos_casa_id_seq;

-- Crear tabla fotos_casa
CREATE TABLE IF NOT EXISTS public.fotos_casa (
  id integer NOT NULL DEFAULT nextval('fotos_casa_id_seq'::regclass),
  casaid integer NOT NULL,
  url text NOT NULL,
  nombre_archivo text NOT NULL,
  fechacreacion timestamp with time zone NOT NULL DEFAULT now(),
  CONSTRAINT fotos_casa_pkey PRIMARY KEY (id),
  CONSTRAINT fk_fotos_casa FOREIGN KEY (casaid) REFERENCES public.casas(id) ON DELETE CASCADE
);

-- Crear índices
CREATE INDEX IF NOT EXISTS idx_fotos_casa_casaid ON public.fotos_casa(casaid);
CREATE INDEX IF NOT EXISTS idx_fotos_casa_fecha ON public.fotos_casa(fechacreacion DESC);

-- Habilitar Row Level Security
ALTER TABLE public.fotos_casa ENABLE ROW LEVEL SECURITY;

-- Policy para usuarios autenticados
CREATE POLICY "Usuarios autenticados pueden ver fotos" ON public.fotos_casa
  FOR SELECT
  USING (auth.role() = 'authenticated');

CREATE POLICY "Usuarios autenticados pueden agregar fotos" ON public.fotos_casa
  FOR INSERT
  WITH CHECK (auth.role() = 'authenticated');

CREATE POLICY "Usuarios autenticados pueden eliminar fotos" ON public.fotos_casa
  FOR DELETE
  USING (auth.role() = 'authenticated');

-- Habilitar realtime
ALTER PUBLICATION supabase_realtime ADD TABLE public.fotos_casa;

-- Comentarios
COMMENT ON TABLE public.fotos_casa IS 'URLs de fotos de las casas almacenadas en Supabase Storage';
COMMENT ON COLUMN public.fotos_casa.url IS 'URL pública de la foto en Storage';
COMMENT ON COLUMN public.fotos_casa.nombre_archivo IS 'Nombre del archivo en Storage';

-- Verificación
SELECT 'Tabla fotos_casa creada exitosamente' AS mensaje;

-- =====================================================
-- CONFIGURACIÓN DEL BUCKET EN SUPABASE
-- =====================================================
-- 
-- IMPORTANTE: Debes configurar el bucket "CasasFotos" en Supabase:
-- 
-- 1. Ve a Storage > CasasFotos
-- 2. Configuración > Políticas
-- 3. Agregar estas políticas:
-- 
-- POLÍTICA DE LECTURA (público):
-- Nombre: Lectura pública de fotos
-- Tipo: SELECT
-- Target roles: public
-- USING expression: true
-- 
-- POLÍTICA DE INSERCIÓN:
-- Nombre: Usuarios autenticados pueden subir
-- Tipo: INSERT
-- Target roles: authenticated
-- WITH CHECK expression: true
-- 
-- POLÍTICA DE ELIMINACIÓN:
-- Nombre: Usuarios autenticados pueden eliminar
-- Tipo: DELETE
-- Target roles: authenticated
-- USING expression: true
-- 
-- =====================================================
