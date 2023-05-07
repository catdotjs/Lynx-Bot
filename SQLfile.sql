CREATE TABLE IF NOT EXISTS polling(
    message_id text NOT NULL,
    vote_data json NOT NULL,
    time_left bigint,
    CONSTRAINT polling_pkey PRIMARY KEY (message_id)
);
