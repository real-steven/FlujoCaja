-- ============================================================
-- Script 11: Tabla movimientos_ia_temporales
-- Almacena los movimientos parseados por IA antes de ser
-- aprobados y pasados a la tabla principal de movimientos.
-- ============================================================

CREATE TABLE IF NOT EXISTS public.movimientos_ia_temporales (
    id              SERIAL PRIMARY KEY,
    casaid          INTEGER NOT NULL REFERENCES public.casas(id) ON DELETE CASCADE,
    hoja_mensual_id INTEGER REFERENCES public.hojas_mensuales(id) ON DELETE SET NULL,
    mes             INTEGER NOT NULL,
    anio            INTEGER NOT NULL,

    -- Datos del movimiento (resultado del parseo IA)
    fecha           DATE,
    monto           NUMERIC(12,2),
    descripcion     TEXT,
    categoria       VARCHAR(255),
    tipo_movimiento VARCHAR(20),         -- 'Ingreso' o 'Gasto'

    -- Imagen de la factura (storage path en bucket privado "facturas")
    factura_url     TEXT,

    -- Estado del registro
    -- 'pendiente': recién importado, esperando revisión
    -- 'parcial': parseo incompleto o con errores
    -- 'aprobado': aceptado y pasado a movimientos
    -- 'error': fallo completo del parseo
    estado          VARCHAR(20) NOT NULL DEFAULT 'pendiente'
        CHECK (estado IN ('pendiente', 'parcial', 'aprobado', 'error')),

    -- Datos crudos de la IA para diagnóstico
    raw_json        JSONB,

    -- Auditoría
    usuario_creador TEXT,
    fecha_creacion  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    usuario_aprobo  TEXT,
    fecha_aprobacion TIMESTAMPTZ,

    -- Indica que el movimiento aprobado que se creó en la tabla principal
    movimiento_id_creado INTEGER REFERENCES public.movimientos(id) ON DELETE SET NULL
);

-- Índices útiles
CREATE INDEX IF NOT EXISTS idx_mov_ia_casaid  ON public.movimientos_ia_temporales(casaid);
CREATE INDEX IF NOT EXISTS idx_mov_ia_estado  ON public.movimientos_ia_temporales(estado);
CREATE INDEX IF NOT EXISTS idx_mov_ia_periodo ON public.movimientos_ia_temporales(casaid, anio, mes);

-- RLS
ALTER TABLE public.movimientos_ia_temporales ENABLE ROW LEVEL SECURITY;

-- Política: solo usuarios autenticados pueden ver/escribir sus propios registros
CREATE POLICY "Autenticados pueden leer movimientos IA"
    ON public.movimientos_ia_temporales
    FOR SELECT
    USING (auth.role() = 'authenticated');

CREATE POLICY "Autenticados pueden insertar movimientos IA"
    ON public.movimientos_ia_temporales
    FOR INSERT
    WITH CHECK (auth.role() = 'authenticated');

CREATE POLICY "Autenticados pueden actualizar movimientos IA"
    ON public.movimientos_ia_temporales
    FOR UPDATE
    USING (auth.role() = 'authenticated');

CREATE POLICY "Autenticados pueden eliminar movimientos IA"
    ON public.movimientos_ia_temporales
    FOR DELETE
    USING (auth.role() = 'authenticated');
