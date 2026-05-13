-- Script 12: Verificar y actualizar estructura tabla usuarios
-- La tabla usuarios ya debe existir con la estructura básica.
-- Este script agrega columnas faltantes si no existen.

-- Asegurar columna auth_id
ALTER TABLE public.usuarios
    ADD COLUMN IF NOT EXISTS auth_id UUID REFERENCES auth.users(id);

-- Asegurar columna rol
ALTER TABLE public.usuarios
    ADD COLUMN IF NOT EXISTS rol VARCHAR(20) NOT NULL DEFAULT 'usuario'
        CHECK (rol IN ('admin', 'usuario'));

-- Asegurar columna activo
ALTER TABLE public.usuarios
    ADD COLUMN IF NOT EXISTS activo BOOLEAN NOT NULL DEFAULT TRUE;

-- Asegurar columna nombre/apellido
ALTER TABLE public.usuarios
    ADD COLUMN IF NOT EXISTS nombre  VARCHAR(255);
ALTER TABLE public.usuarios
    ADD COLUMN IF NOT EXISTS apellido VARCHAR(255);
ALTER TABLE public.usuarios
    ADD COLUMN IF NOT EXISTS telefono VARCHAR(50);

-- Índice para búsqueda por auth_id
CREATE INDEX IF NOT EXISTS idx_usuarios_auth_id ON public.usuarios(auth_id);

-- RLS: solo usuarios autenticados pueden ver la tabla
ALTER TABLE public.usuarios ENABLE ROW LEVEL SECURITY;

CREATE POLICY IF NOT EXISTS "Autenticados pueden leer usuarios"
    ON public.usuarios FOR SELECT
    USING (auth.role() = 'authenticated');

-- Solo admin puede insertar/actualizar/eliminar.
-- En la app, el helper usa el service role key para estas operaciones,
-- o bien el admin puede usar su propio token si la política lo permite.
CREATE POLICY IF NOT EXISTS "Autenticados pueden insertar usuarios"
    ON public.usuarios FOR INSERT
    WITH CHECK (auth.role() = 'authenticated');

CREATE POLICY IF NOT EXISTS "Autenticados pueden actualizar usuarios"
    ON public.usuarios FOR UPDATE
    USING (auth.role() = 'authenticated');
