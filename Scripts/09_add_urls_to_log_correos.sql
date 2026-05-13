-- =============================================
-- Script: Agregar columnas de URLs a log_correos
-- Descripción: Guarda las URLs públicas de Storage para PDF y Excel
-- =============================================

ALTER TABLE public.log_correos
    ADD COLUMN IF NOT EXISTS url_pdf TEXT,
    ADD COLUMN IF NOT EXISTS url_excel TEXT;

COMMENT ON COLUMN public.log_correos.url_pdf   IS 'URL pública del PDF en Supabase Storage (bucket: reportes)';
COMMENT ON COLUMN public.log_correos.url_excel IS 'URL pública del Excel en Supabase Storage (bucket: reportes)';
