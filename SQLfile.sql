CREATE TABLE IF NOT EXISTS public.polling
(
    message_id text COLLATE pg_catalog."default" NOT NULL,
    options_and_votes json NOT NULL,
    time_left bigint,
    voted text[] COLLATE pg_catalog."default",
    CONSTRAINT polling_pkey PRIMARY KEY (message_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.polling
    OWNER to postgres;