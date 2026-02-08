-- Script para crear tabla de preferencias de usuario
-- Ejecutar en Supabase SQL Editor

-- Crear tabla de preferencias
CREATE TABLE IF NOT EXISTS preferencias_usuario (
    id SERIAL PRIMARY KEY,
    usuario_email VARCHAR(255) NOT NULL UNIQUE,
    modo_oscuro BOOLEAN DEFAULT FALSE,
    fecha_creacion TIMESTAMP DEFAULT NOW(),
    fecha_actualizacion TIMESTAMP DEFAULT NOW(),
    CONSTRAINT fk_usuario FOREIGN KEY (usuario_email) REFERENCES auth.users(email) ON DELETE CASCADE
);

-- Índice para búsquedas rápidas por usuario
CREATE INDEX IF NOT EXISTS idx_preferencias_usuario_email ON preferencias_usuario(usuario_email);

-- Función para actualizar fecha_actualizacion automáticamente
CREATE OR REPLACE FUNCTION actualizar_fecha_actualizacion_preferencias()
RETURNS TRIGGER AS $$
BEGIN
    NEW.fecha_actualizacion = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger para actualizar fecha_actualizacion
DROP TRIGGER IF EXISTS trigger_actualizar_preferencias ON preferencias_usuario;
CREATE TRIGGER trigger_actualizar_preferencias
    BEFORE UPDATE ON preferencias_usuario
    FOR EACH ROW
    EXECUTE FUNCTION actualizar_fecha_actualizacion_preferencias();

-- Comentarios
COMMENT ON TABLE preferencias_usuario IS 'Tabla para almacenar preferencias de usuario del sistema';
COMMENT ON COLUMN preferencias_usuario.modo_oscuro IS 'TRUE = Modo oscuro activado, FALSE = Modo claro';
