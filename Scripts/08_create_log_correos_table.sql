-- =============================================
-- Script: Crear tabla de log de correos enviados
-- Descripción: Registra todos los correos enviados desde el sistema
-- =============================================

CREATE TABLE IF NOT EXISTS public.log_correos (
    id SERIAL PRIMARY KEY,
    usuario_email VARCHAR(255) NOT NULL,
    casa_nombre VARCHAR(255) NOT NULL,
    destinatarios TEXT NOT NULL,         -- emails separados por coma
    asunto TEXT NOT NULL,
    adjuntos VARCHAR(50) NOT NULL,       -- 'pdf', 'excel', 'pdf+excel'
    mensaje_id VARCHAR(255),             -- ID devuelto por Resend
    estado VARCHAR(20) NOT NULL,         -- 'enviado', 'error'
    error_detalle TEXT,                  -- solo si estado = 'error'
    fecha TIMESTAMPTZ DEFAULT now() NOT NULL
);

-- Índices
CREATE INDEX IF NOT EXISTS idx_log_correos_usuario ON public.log_correos(usuario_email);
CREATE INDEX IF NOT EXISTS idx_log_correos_fecha ON public.log_correos(fecha DESC);
CREATE INDEX IF NOT EXISTS idx_log_correos_estado ON public.log_correos(estado);
CREATE INDEX IF NOT EXISTS idx_log_correos_casa ON public.log_correos(casa_nombre);

-- Comentarios
COMMENT ON TABLE public.log_correos IS 'Log de correos enviados desde el sistema';
COMMENT ON COLUMN public.log_correos.destinatarios IS 'Lista de destinatarios separados por coma';
COMMENT ON COLUMN public.log_correos.adjuntos IS 'Archivos adjuntos: pdf, excel o pdf+excel';
COMMENT ON COLUMN public.log_correos.mensaje_id IS 'ID del mensaje devuelto por Resend';
COMMENT ON COLUMN public.log_correos.estado IS 'Estado del envío: enviado o error';

-- Habilitar Row Level Security
ALTER TABLE public.log_correos ENABLE ROW LEVEL SECURITY;

-- Política: Los usuarios autenticados pueden leer
CREATE POLICY "Usuarios autenticados pueden leer log_correos"
ON public.log_correos
FOR SELECT
TO authenticated
USING (true);

-- Política: Los usuarios autenticados pueden insertar
CREATE POLICY "Usuarios autenticados pueden insertar log_correos"
ON public.log_correos
FOR INSERT
TO authenticated
WITH CHECK (true);
