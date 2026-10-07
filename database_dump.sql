--
-- PostgreSQL database dump
--

\restrict s3WPB0kxA35ljbhpQqto1jGTXVtrikmDU341sJlZO4JPyniAGYJunETrwyeKPma

-- Dumped from database version 18.1
-- Dumped by pg_dump version 18.1

-- Started on 2026-10-07 20:53:40

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 220 (class 1259 OID 33015)
-- Name: Tasks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Tasks" (
    "Id" uuid NOT NULL,
    "Title" character varying(100) NOT NULL,
    "IsCompleted" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "IsDeleted" boolean NOT NULL
);


ALTER TABLE public."Tasks" OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 33008)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- TOC entry 4880 (class 0 OID 33015)
-- Dependencies: 220
-- Data for Name: Tasks; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Tasks" ("Id", "Title", "IsCompleted", "CreatedAt", "IsDeleted") FROM stdin;
3ccad223-fb93-462a-8d14-f6492363e2aa	Выпить кофе.	t	2026-10-06 14:42:00+03	t
01ec9730-9025-42d9-8f07-1962fe955e7c	Устроиться на работу	f	2026-10-07 09:26:40.343071+03	f
\.


--
-- TOC entry 4879 (class 0 OID 33008)
-- Dependencies: 219
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20261006144213_InitialTasksTable	10.0.12
\.


--
-- TOC entry 4731 (class 2606 OID 33024)
-- Name: Tasks PK_Tasks; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Tasks"
    ADD CONSTRAINT "PK_Tasks" PRIMARY KEY ("Id");


--
-- TOC entry 4729 (class 2606 OID 33014)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


-- Completed on 2026-10-07 20:53:40

--
-- PostgreSQL database dump complete
--

\unrestrict s3WPB0kxA35ljbhpQqto1jGTXVtrikmDU341sJlZO4JPyniAGYJunETrwyeKPma

