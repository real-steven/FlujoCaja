-- ============================================
-- SCRIPT COMPLETO DE INICIALIZACIÓN V2
-- Sistema de Flujo de Caja - Supabase
-- Con auditoría mejorada y sin tabla usuarios
-- ============================================

-- ============================================
-- 1. ELIMINAR TABLAS EXISTENTES (si existen)
-- ============================================

DROP TABLE IF EXISTS public.movimientos CASCADE;
DROP TABLE IF EXISTS public.hojas_mensuales CASCADE;
DROP TABLE IF EXISTS public.casas CASCADE;
DROP TABLE IF EXISTS public.categorias_movimientos CASCADE;
DROP TABLE IF EXISTS public.categorias CASCADE;
DROP TABLE IF EXISTS public.duenos CASCADE;

-- ============================================
-- 2. CREAR SECUENCIAS
-- ============================================

CREATE SEQUENCE IF NOT EXISTS public.duenos_id_seq;
CREATE SEQUENCE IF NOT EXISTS public.categorias_id_seq;
CREATE SEQUENCE IF NOT EXISTS public.categorias_movimientos_id_seq;
CREATE SEQUENCE IF NOT EXISTS public.casas_id_seq;
CREATE SEQUENCE IF NOT EXISTS public.hojas_mensuales_id_seq;
CREATE SEQUENCE IF NOT EXISTS public.movimientos_id_seq;
CREATE SEQUENCE IF NOT EXISTS public.notas_casa_id_seq;
CREATE SEQUENCE IF NOT EXISTS public.fotos_casa_id_seq;

-- ============================================
-- 3. CREAR TABLAS
-- ============================================

-- Tabla dueños
CREATE TABLE public.duenos (
  id bigint NOT NULL DEFAULT nextval('duenos_id_seq'::regclass),
  nombre text NOT NULL,
  apellido text NOT NULL,
  telefono text,
  email text,
  fecha_creacion timestamp with time zone DEFAULT now(),
  fecha_actualizacion timestamp with time zone DEFAULT now(),
  activo boolean DEFAULT true,
  NombreCompleto text,
  CONSTRAINT duenos_pkey PRIMARY KEY (id)
);

-- Tabla categorías (de casas)
CREATE TABLE public.categorias (
  id integer NOT NULL DEFAULT nextval('categorias_id_seq'::regclass),
  nombre text NOT NULL UNIQUE,
  descripcion text,
  fechacreacion timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
  activo boolean DEFAULT true,
  CONSTRAINT categorias_pkey PRIMARY KEY (id)
);

-- Tabla categorías de movimientos
CREATE TABLE public.categorias_movimientos (
  id integer NOT NULL DEFAULT nextval('categorias_movimientos_id_seq'::regclass),
  nombre character varying NOT NULL UNIQUE,
  descripcion text,
  tipo character varying NOT NULL CHECK (tipo IN ('ingreso', 'egreso')),
  activo boolean NOT NULL DEFAULT true,
  fechacreacion timestamp with time zone NOT NULL DEFAULT now(),
  CONSTRAINT categorias_movimientos_pkey PRIMARY KEY (id)
);

-- Tabla casas
CREATE TABLE public.casas (
  id integer NOT NULL DEFAULT nextval('casas_id_seq'::regclass),
  nombre text NOT NULL,
  activo boolean NOT NULL DEFAULT true,
  duenoid integer NOT NULL,
  categoriaid integer NOT NULL,
  rutaimagen text,
  notas text,
  fechacreacion timestamp with time zone NOT NULL DEFAULT now(),
  moneda character varying NOT NULL DEFAULT 'USD'::character varying CHECK (moneda::text = ANY (ARRAY['USD'::character varying, 'CRC'::character varying, 'EUR'::character varying]::text[])),
  CONSTRAINT casas_pkey PRIMARY KEY (id),
  CONSTRAINT fk_casas_dueno FOREIGN KEY (duenoid) REFERENCES public.duenos(id) ON DELETE RESTRICT,
  CONSTRAINT fk_casas_categoria FOREIGN KEY (categoriaid) REFERENCES public.categorias(id) ON DELETE RESTRICT
);

-- Tabla hojas mensuales (para cierres mensuales de movimientos)
CREATE TABLE public.hojas_mensuales (
  id integer NOT NULL DEFAULT nextval('hojas_mensuales_id_seq'::regclass),
  casaid integer NOT NULL,
  mes integer NOT NULL CHECK (mes >= 1 AND mes <= 12),
  anio integer NOT NULL CHECK (anio >= 2020),
  fechacreacion timestamp with time zone DEFAULT now(),
  cerrada boolean DEFAULT false,
  CONSTRAINT hojas_mensuales_pkey PRIMARY KEY (id),
  CONSTRAINT hojas_mensuales_casaid_fkey FOREIGN KEY (casaid) REFERENCES public.casas(id) ON DELETE CASCADE,
  CONSTRAINT hojas_mensuales_unique UNIQUE (casaid, mes, anio)
);

-- Tabla movimientos (con auditoría mejorada usando auth.users)
CREATE TABLE public.movimientos (
  id integer NOT NULL DEFAULT nextval('movimientos_id_seq'::regclass),
  casaid integer NOT NULL,
  hoja_mensual_id integer,
  fecha date NOT NULL,
  descripcion text NOT NULL CHECK (length(TRIM(BOTH FROM descripcion)) > 0),
  monto numeric NOT NULL CHECK (monto <> 0::numeric),
  categoria text NOT NULL CHECK (length(TRIM(BOTH FROM categoria)) > 0),
  fechacreacion timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
  activo boolean DEFAULT true,
  tipo_movimiento text DEFAULT 'Ingreso'::text CHECK (tipo_movimiento = ANY (ARRAY['Ingreso'::text, 'Gasto'::text])),
  usuario_creador_id uuid REFERENCES auth.users(id),
  fechamodificacion timestamp with time zone DEFAULT now(),
  usuario_modificador_id uuid REFERENCES auth.users(id),
  CONSTRAINT movimientos_pkey PRIMARY KEY (id),
  CONSTRAINT fk_movimientos_casa FOREIGN KEY (casaid) REFERENCES public.casas(id) ON DELETE CASCADE,
  CONSTRAINT fk_movimientos_hoja_mensual FOREIGN KEY (hoja_mensual_id) REFERENCES public.hojas_mensuales(id) ON DELETE SET NULL
);

-- Tabla notas de casas
CREATE TABLE public.notas_casa (
  id integer NOT NULL DEFAULT nextval('notas_casa_id_seq'::regclass),
  casaid integer NOT NULL,
  contenido text NOT NULL,
  fechacreacion timestamp with time zone NOT NULL DEFAULT now(),
  CONSTRAINT notas_casa_pkey PRIMARY KEY (id),
  CONSTRAINT fk_notas_casa FOREIGN KEY (casaid) REFERENCES public.casas(id) ON DELETE CASCADE
);

-- Tabla fotos de casas
CREATE TABLE public.fotos_casa (
  id integer NOT NULL DEFAULT nextval('fotos_casa_id_seq'::regclass),
  casaid integer NOT NULL,
  url text NOT NULL,
  nombre_archivo text NOT NULL,
  fechacreacion timestamp with time zone NOT NULL DEFAULT now(),
  CONSTRAINT fotos_casa_pkey PRIMARY KEY (id),
  CONSTRAINT fk_fotos_casa FOREIGN KEY (casaid) REFERENCES public.casas(id) ON DELETE CASCADE
);

-- Tabla de auditoría del sistema
CREATE TABLE IF NOT EXISTS public.auditoria (
  id SERIAL PRIMARY KEY,
  usuario_email VARCHAR(255) NOT NULL,
  modulo VARCHAR(50) NOT NULL,
  tipo_accion VARCHAR(50) NOT NULL,
  entidad_id INT,
  entidad_nombre VARCHAR(255),
  descripcion TEXT NOT NULL,
  datos_anteriores JSONB,
  datos_nuevos JSONB,
  fecha TIMESTAMPTZ DEFAULT now() NOT NULL
);

-- ============================================
-- 4. CREAR ÍNDICES
-- ============================================

CREATE INDEX IF NOT EXISTS idx_casas_activo ON public.casas(activo);
CREATE INDEX IF NOT EXISTS idx_casas_dueno ON public.casas(duenoid);
CREATE INDEX IF NOT EXISTS idx_casas_notas ON public.casas USING gin(to_tsvector('spanish', notas));
CREATE INDEX IF NOT EXISTS idx_hojas_mensuales_casa ON public.hojas_mensuales(casaid);
CREATE INDEX IF NOT EXISTS idx_hojas_mensuales_anio_mes ON public.hojas_mensuales(anio, mes);
CREATE INDEX IF NOT EXISTS idx_movimientos_casa_fecha ON public.movimientos(casaid, fecha DESC);
CREATE INDEX IF NOT EXISTS idx_movimientos_tipo ON public.movimientos(tipo_movimiento);
CREATE INDEX IF NOT EXISTS idx_movimientos_usuario_creador ON public.movimientos(usuario_creador_id);
CREATE INDEX IF NOT EXISTS idx_movimientos_hoja_mensual ON public.movimientos(hoja_mensual_id);
CREATE INDEX IF NOT EXISTS idx_notas_casa_casaid ON public.notas_casa(casaid);
CREATE INDEX IF NOT EXISTS idx_notas_casa_fecha ON public.notas_casa(fechacreacion DESC);
CREATE INDEX IF NOT EXISTS idx_fotos_casa_casaid ON public.fotos_casa(casaid);
CREATE INDEX IF NOT EXISTS idx_fotos_casa_fecha ON public.fotos_casa(fechacreacion DESC);
CREATE INDEX IF NOT EXISTS idx_auditoria_modulo ON public.auditoria(modulo);
CREATE INDEX IF NOT EXISTS idx_auditoria_usuario ON public.auditoria(usuario_email);
CREATE INDEX IF NOT EXISTS idx_auditoria_fecha ON public.auditoria(fecha DESC);
CREATE INDEX IF NOT EXISTS idx_auditoria_entidad ON public.auditoria(modulo, entidad_id);
CREATE INDEX IF NOT EXISTS idx_auditoria_tipo_accion ON public.auditoria(tipo_accion);

-- ============================================
-- 5. CREAR TRIGGERS PARA AUDITORÍA
-- ============================================

-- Función para actualizar fecha de modificación automáticamente
CREATE OR REPLACE FUNCTION actualizar_fecha_modificacion()
RETURNS TRIGGER AS $$
BEGIN
  NEW.fechamodificacion = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger para movimientos
CREATE TRIGGER trigger_actualizar_fecha_modificacion_movimientos
BEFORE UPDATE ON public.movimientos
FOR EACH ROW EXECUTE FUNCTION actualizar_fecha_modificacion();

-- Trigger para duenos
CREATE TRIGGER trigger_actualizar_fecha_modificacion_duenos
BEFORE UPDATE ON public.duenos
FOR EACH ROW EXECUTE FUNCTION actualizar_fecha_modificacion();

-- ============================================
-- 6. INSERTAR DATOS DE PRUEBA
-- ============================================

-- Dueños
INSERT INTO public.duenos (nombre, apellido, telefono, email, activo, NombreCompleto) VALUES 
  ('Juan', 'Pérez García', '+506 8888-1111', 'juan.perez@example.com', true, 'Juan Pérez García'),
  ('María', 'González López', '+506 8888-2222', 'maria.gonzalez@example.com', true, 'María González López'),
  ('Carlos', 'Rodríguez Mora', '+506 8888-3333', 'carlos.rodriguez@example.com', true, 'Carlos Rodríguez Mora'),
  ('Ana', 'Martínez Soto', '+506 8888-4444', 'ana.martinez@example.com', true, 'Ana Martínez Soto'),
  ('Luis', 'Hernández Cruz', '+506 8888-5555', 'luis.hernandez@example.com', true, 'Luis Hernández Cruz');

-- Categorías de casas
INSERT INTO public.categorias (nombre, descripcion, activo) VALUES 
  ('Residencial', 'Propiedades residenciales para alquiler', true),
  ('Comercial', 'Locales comerciales y oficinas', true),
  ('Vacacional', 'Propiedades para alquiler vacacional', true),
  ('Bodega', 'Bodegas y espacios de almacenamiento', true),
  ('Terreno', 'Terrenos y lotes', true);

-- Categorías de movimientos
INSERT INTO public.categorias_movimientos (nombre, descripcion, tipo, activo) VALUES 
  ('Alquiler Mensual', 'Pago de alquiler mensual', 'ingreso', true),
  ('Electricidad', 'Pago de servicio eléctrico', 'egreso', true),
  ('Agua', 'Pago de servicio de agua', 'egreso', true),
  ('Reparaciones', 'Gastos de mantenimiento y reparaciones', 'egreso', true),
  ('Impuestos', 'Pago de impuestos municipales', 'egreso', true);

-- Casas (usando IDs de dueños y categorías)
INSERT INTO public.casas (nombre, activo, duenoid, categoriaid, moneda) VALUES 
  ('Casa Los Ángeles', true, 1, 1, 'USD'),
  ('Apartamento Centro', true, 2, 1, 'CRC'),
  ('Local Comercial Plaza', true, 3, 2, 'USD'),
  ('Casa de Playa Guanacaste', true, 4, 3, 'USD'),
  ('Oficinas Ejecutivas Torre', true, 5, 2, 'USD');

-- Movimientos de prueba (sin usuario_creador_id ya que aún no hay usuarios creados en auth.users)
INSERT INTO public.movimientos (casaid, fecha, descripcion, monto, categoria, tipo_movimiento, activo) VALUES 
  (1, CURRENT_DATE - INTERVAL '30 days', 'Alquiler mensual enero', 800.00, 'Alquiler Mensual', 'Ingreso', true),
  (1, CURRENT_DATE - INTERVAL '25 days', 'Pago de electricidad', -120.50, 'Electricidad', 'Gasto', true),
  (1, CURRENT_DATE - INTERVAL '20 days', 'Pago de agua', -45.00, 'Agua', 'Gasto', true),
  (2, CURRENT_DATE - INTERVAL '15 days', 'Alquiler mensual', 650000.00, 'Alquiler Mensual', 'Ingreso', true),
  (2, CURRENT_DATE - INTERVAL '10 days', 'Reparación de tubería', -85000.00, 'Reparaciones', 'Gasto', true);

-- ============================================
-- 7. CREAR HOJAS MENSUALES PARA CADA CASA
-- ============================================

-- Crear hojas de todos los meses de 2025 y enero/febrero 2026 para cada casa
DO $$ 
DECLARE
  casa_record RECORD;
  mes_num integer;
BEGIN
  FOR casa_record IN SELECT id FROM public.casas LOOP
    -- Crear hojas de enero a diciembre 2025
    FOR mes_num IN 1..12 LOOP
      INSERT INTO public.hojas_mensuales (casaid, mes, anio, cerrada)
      VALUES (casa_record.id, mes_num, 2025, false)
      ON CONFLICT (casaid, mes, anio) DO NOTHING;
    END LOOP;
    
    -- Crear hojas de enero y febrero 2026
    INSERT INTO public.hojas_mensuales (casaid, mes, anio, cerrada)
    VALUES (casa_record.id, 1, 2026, false)
    ON CONFLICT (casaid, mes, anio) DO NOTHING;
    
    INSERT INTO public.hojas_mensuales (casaid, mes, anio, cerrada)
    VALUES (casa_record.id, 2, 2026, false)
    ON CONFLICT (casaid, mes, anio) DO NOTHING;
  END LOOP;
END $$;

-- Asignar movimientos existentes a la hoja de Enero 2026
UPDATE public.movimientos m
SET hoja_mensual_id = (
  SELECT hm.id 
  FROM public.hojas_mensuales hm 
  WHERE hm.casaid = m.casaid 
  AND hm.mes = 1 
  AND hm.anio = 2026
  LIMIT 1
)
WHERE m.hoja_mensual_id IS NULL;

-- ============================================
-- 8. HABILITAR ROW LEVEL SECURITY (RLS)
-- ============================================

ALTER TABLE public.casas ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.duenos ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.categorias ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.categorias_movimientos ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.hojas_mensuales ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.movimientos ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.notas_casa ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.fotos_casa ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.auditoria ENABLE ROW LEVEL SECURITY;

-- Políticas: Usuarios autenticados tienen acceso completo
CREATE POLICY "Acceso completo casas" ON public.casas FOR ALL USING (auth.role() = 'authenticated');
CREATE POLICY "Acceso completo duenos" ON public.duenos FOR ALL USING (auth.role() = 'authenticated');
CREATE POLICY "Acceso completo categorias" ON public.categorias FOR ALL USING (auth.role() = 'authenticated');
CREATE POLICY "Acceso completo categorias_movimientos" ON public.categorias_movimientos FOR ALL USING (auth.role() = 'authenticated');
CREATE POLICY "Acceso completo hojas_mensuales" ON public.hojas_mensuales FOR ALL USING (auth.role() = 'authenticated');
CREATE POLICY "Acceso completo movimientos" ON public.movimientos FOR ALL USING (auth.role() = 'authenticated');
CREATE POLICY "Acceso completo notas_casa" ON public.notas_casa FOR ALL USING (auth.role() = 'authenticated');
CREATE POLICY "Acceso completo fotos_casa" ON public.fotos_casa FOR ALL USING (auth.role() = 'authenticated');
CREATE POLICY "Usuarios autenticados pueden leer auditoría" ON public.auditoria FOR SELECT TO authenticated USING (true);
CREATE POLICY "Usuarios autenticados pueden crear auditoría" ON public.auditoria FOR INSERT TO authenticated WITH CHECK (true);

-- ============================================
-- 9. HABILITAR REALTIME
-- ============================================

ALTER PUBLICATION supabase_realtime ADD TABLE public.casas;
ALTER PUBLICATION supabase_realtime ADD TABLE public.duenos;
ALTER PUBLICATION supabase_realtime ADD TABLE public.hojas_mensuales;
ALTER PUBLICATION supabase_realtime ADD TABLE public.movimientos;
ALTER PUBLICATION supabase_realtime ADD TABLE public.notas_casa;
ALTER PUBLICATION supabase_realtime ADD TABLE public.fotos_casa;
ALTER PUBLICATION supabase_realtime ADD TABLE public.auditoria;

-- ============================================
-- 10. VERIFICACIÓN
-- ============================================

SELECT 'Dueños insertados:' as tabla, COUNT(*) as total FROM public.duenos;
SELECT 'Categorías insertadas:' as tabla, COUNT(*) as total FROM public.categorias;
SELECT 'Categorías movimientos:' as tabla, COUNT(*) as total FROM public.categorias_movimientos;
SELECT 'Casas insertadas:' as tabla, COUNT(*) as total FROM public.casas;
SELECT 'Hojas mensuales creadas:' as tabla, COUNT(*) as total FROM public.hojas_mensuales;
SELECT 'Movimientos insertados:' as tabla, COUNT(*) as total FROM public.movimientos;

-- Ver casas con sus dueños
SELECT 
  c.id,
  c.nombre as casa,
  c.moneda,
  c.notas,
  d.NombreCompleto as dueno,
  cat.nombre as categoria,
  c.activo
FROM public.casas c
JOIN public.duenos d ON c.duenoid = d.id
JOIN public.categorias cat ON c.categoriaid = cat.id
ORDER BY c.nombre;

-- Ver resumen de hojas mensuales por casa
SELECT 
  c.nombre as casa,
  hm.anio,
  COUNT(*) as meses_creados
FROM public.hojas_mensuales hm
JOIN public.casas c ON hm.casaid = c.id
GROUP BY c.nombre, hm.anio
ORDER BY c.nombre, hm.anio;

-- Ver movimientos con su hoja mensual
SELECT 
  c.nombre as casa,
  hm.mes,
  hm.anio,
  COUNT(m.id) as movimientos
FROM public.hojas_mensuales hm
JOIN public.casas c ON hm.casaid = c.id
LEFT JOIN public.movimientos m ON m.hoja_mensual_id = hm.id
WHERE hm.anio = 2026
GROUP BY c.nombre, hm.mes, hm.anio
ORDER BY c.nombre, hm.mes;

-- ============================================
-- FIN DEL SCRIPT
-- ============================================

-- ═══════════════════════════════════════════
-- NOTAS IMPORTANTES
-- ═══════════════════════════════════════════
-- 
-- 1. USUARIOS: Se crean en Supabase Dashboard → Authentication → Users
--    No existe tabla 'usuarios' personalizada. Se usa auth.users nativo.
--
-- 2. AUDITORÍA: 
--    - usuario_creador_id: UUID de quien creó el registro (auth.users)
--    - usuario_modificador_id: UUID de quien modificó (auth.users)
--    - fechacreacion: Cuándo se creó
--    - fechamodificacion: Se actualiza automáticamente con trigger
--
-- 3. RELACIONES CASCADE:
--    - Eliminar CASA → Elimina MOVIMIENTOS y HOJAS_MENSUALES (CASCADE)
--    - Eliminar HOJA_MENSUAL → Movimientos quedan sin asignar (SET NULL)
--    - Eliminar DUEÑO → Bloqueado si tiene casas (RESTRICT)
--    - Eliminar CATEGORÍA → Bloqueado si tiene casas (RESTRICT)
--
-- 4. PARA REPORTES DE AUDITORÍA:
--    SELECT m.*, u.email as usuario_email
--    FROM movimientos m
--    LEFT JOIN auth.users u ON m.usuario_creador_id = u.id;
--
-- 5. MONEDAS SOPORTADAS: USD, CRC, EUR
--
-- 6. ROW LEVEL SECURITY: Solo usuarios autenticados pueden acceder
--
-- 7. REALTIME: Activado para casas, duenos, hojas_mensuales y movimientos
--
-- 8. CAMPO NOTAS en CASAS:
--    - Para dueños secundarios/terciarios (ej: "Dueño 2: María López")
--    - Observaciones generales de la propiedad
--    - Información adicional relevante
--    - Incluye índice de búsqueda full-text en español
--
-- 9. HOJAS MENSUALES:
--    - Se crean automáticamente para cada casa
--    - 2025: 12 meses completos (Enero a Diciembre)
--    - 2026: Enero y Febrero (mes actual + siguiente)
--    - Constraint UNIQUE evita duplicados (casaid + mes + anio)
--
-- 10. ASIGNACIÓN DE MOVIMIENTOS:
--     - Los movimientos se asignan a hojas mensuales por ID, NO por fecha
--     - Ejemplo: Movimiento con fecha 15/12/2025 puede estar en hoja Enero 2026
--     - Esto permite registrar facturas atrasadas en el cierre del mes actual
