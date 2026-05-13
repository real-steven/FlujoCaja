-- Script para agregar soporte de imágenes en movimientos
-- Ejecutar en Supabase SQL Editor

ALTER TABLE movimientos
ADD COLUMN IF NOT EXISTS imagen_url TEXT NULL;

COMMENT ON COLUMN movimientos.imagen_url IS 'URL pública de la imagen adjunta al movimiento (almacenada en Supabase Storage bucket CasasFotos)';
