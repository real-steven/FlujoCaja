-- ============================================
-- MIGRACIÓN 07: Mensaje personalizado de reporte por casa
-- Ejecutar en: Supabase Dashboard → SQL Editor
-- ============================================

-- Agregar columna nullable para el mensaje personalizado del correo de reporte
ALTER TABLE public.casas
    ADD COLUMN IF NOT EXISTS mensaje_reporte TEXT;
