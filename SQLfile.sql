CREATE TABLE IF NOT EXISTS polling(
    message_id text NOT NULL,
    options_and_votes json NOT NULL,
    time_left bigint,
    voted text[],
    CONSTRAINT polling_pkey PRIMARY KEY (message_id)
);
