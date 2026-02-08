-- =============================================
-- Script: Crear tabla de auditoría del sistema
-- Descripción: Registra todas las acciones realizadas en el sistema
-- =============================================

-- Crear tabla de auditoría
CREATE TABLE IF NOT EXISTS public.auditoria (
    id SERIAL PRIMARY KEY,
    usuario_email VARCHAR(255) NOT NULL,
    modulo VARCHAR(50) NOT NULL, -- 'casa', 'movimiento', 'dueno', 'categoria', etc.
    tipo_accion VARCHAR(50) NOT NULL, -- 'crear', 'editar', 'eliminar', 'activar', 'desactivar'
    entidad_id INT, -- ID de la entidad afectada
    entidad_nombre VARCHAR(255), -- Nombre para búsqueda rápida
    descripcion TEXT NOT NULL, -- Descripción detallada
    datos_anteriores JSONB, -- Estado antes del cambio
    datos_nuevos JSONB, -- Estado después del cambio
    fecha TIMESTAMPTZ DEFAULT now() NOT NULL
);

-- Crear índices para mejorar rendimiento
CREATE INDEX IF NOT EXISTS idx_auditoria_modulo ON public.auditoria(modulo);
CREATE INDEX IF NOT EXISTS idx_auditoria_usuario ON public.auditoria(usuario_email);
CREATE INDEX IF NOT EXISTS idx_auditoria_fecha ON public.auditoria(fecha DESC);
CREATE INDEX IF NOT EXISTS idx_auditoria_entidad ON public.auditoria(modulo, entidad_id);
CREATE INDEX IF NOT EXISTS idx_auditoria_tipo_accion ON public.auditoria(tipo_accion);

-- Comentarios para documentación
COMMENT ON TABLE public.auditoria IS 'Registro de auditoría de todas las acciones del sistema';
COMMENT ON COLUMN public.auditoria.modulo IS 'Módulo del sistema: casa, movimiento, dueno, categoria';
COMMENT ON COLUMN public.auditoria.tipo_accion IS 'Tipo de acción: crear, editar, eliminar, activar, desactivar';
COMMENT ON COLUMN public.auditoria.entidad_id IS 'ID de la entidad afectada (casa_id, movimiento_id, etc)';
COMMENT ON COLUMN public.auditoria.entidad_nombre IS 'Nombre de la entidad para búsqueda';
COMMENT ON COLUMN public.auditoria.datos_anteriores IS 'Estado anterior en formato JSON';
COMMENT ON COLUMN public.auditoria.datos_nuevos IS 'Estado nuevo en formato JSON';

-- Habilitar Row Level Security
ALTER TABLE public.auditoria ENABLE ROW LEVEL SECURITY;

-- Política: Los usuarios autenticados pueden leer auditorías
CREATE POLICY "Usuarios autenticados pueden leer auditoría"
ON public.auditoria
FOR SELECT
TO authenticated
USING (true);

-- Política: Los usuarios autenticados pueden insertar auditorías
CREATE POLICY "Usuarios autenticados pueden crear auditoría"
ON public.auditoria
FOR INSERT
TO authenticated
WITH CHECK (true);

-- Otorgar permisos
GRANT SELECT, INSERT ON public.auditoria TO authenticated;
GRANT USAGE, SELECT ON SEQUENCE public.auditoria_id_seq TO authenticated;
