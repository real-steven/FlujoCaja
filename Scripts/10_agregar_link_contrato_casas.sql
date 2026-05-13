-- Script: 10_agregar_link_contrato_casas.sql
-- Descripción: Agrega columna link_contrato a la tabla casas
-- Permite almacenar una URL de internet o ruta de archivo (PDF, Excel, Word)

ALTER TABLE casas
ADD COLUMN IF NOT EXISTS link_contrato TEXT;

COMMENT ON COLUMN casas.link_contrato IS 'URL o ruta del contrato de la propiedad (PDF, Excel, Word o link de internet)';
