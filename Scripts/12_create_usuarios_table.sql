-- Script 12: Crear tabla usuarios desde cero
-- Ejecutar en Supabase SQL Editor

CREATE TABLE IF NOT EXISTS public.usuarios (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    auth_id     UUID UNIQUE REFERENCES auth.users(id) ON DELETE CASCADE,
    nombre      VARCHAR(255),
    apellido    VARCHAR(255),
    email       VARCHAR(255) NOT NULL,
    telefono    VARCHAR(50),
    rol         VARCHAR(20) NOT NULL DEFAULT 'usuario'
                    CHECK (rol IN ('admin', 'usuario')),
    activo      BOOLEAN NOT NULL DEFAULT TRUE,
    fecha_creacion TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_usuarios_auth_id ON public.usuarios(auth_id);
CREATE INDEX IF NOT EXISTS idx_usuarios_email  ON public.usuarios(email);

ALTER TABLE public.usuarios ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Autenticados pueden leer usuarios"
    ON public.usuarios FOR SELECT
    USING (auth.role() = 'authenticated');

CREATE POLICY "Autenticados pueden insertar usuarios"
    ON public.usuarios FOR INSERT
    WITH CHECK (auth.role() = 'authenticated');

CREATE POLICY "Autenticados pueden actualizar usuarios"
    ON public.usuarios FOR UPDATE
    USING (auth.role() = 'authenticated');
