create table audit_logs
(
    id          bigint auto_increment
        primary key,
    user_id     bigint                              null,
    action      varchar(50)                         not null,
    entity_type varchar(50)                         not null,
    entity_id   bigint                              not null,
    old_values  json                                null,
    new_values  json                                null,
    ip_address  varchar(45)                         null,
    user_agent  text                                null,
    created_at  timestamp default CURRENT_TIMESTAMP null
);

create index idx_created_at
    on audit_logs (created_at);

create index idx_entity
    on audit_logs (entity_type, entity_id);

create index idx_user_id
    on audit_logs (user_id);

create table categories
(
    id          bigint auto_increment
        primary key,
    name        varchar(50)                         not null,
    slug        varchar(60)                         not null,
    description text                                null,
    parent_id   bigint                              null,
    created_at  timestamp default CURRENT_TIMESTAMP null,
    updated_at  timestamp default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint slug
        unique (slug),
    constraint categories_ibfk_1
        foreign key (parent_id) references categories (id)
            on delete set null
);

create index idx_slug
    on categories (slug);

create index parent_id
    on categories (parent_id);

create table email_subscribers
(
    id         bigint auto_increment
        primary key,
    email      varchar(100)                                                             not null,
    first_name varchar(50)                                                              null,
    last_name  varchar(50)                                                              null,
    status     enum ('subscribed', 'unsubscribed', 'pending') default 'pending'         null,
    token      varchar(100)                                                             null,
    created_at timestamp                                      default CURRENT_TIMESTAMP null,
    updated_at timestamp                                      default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint email
        unique (email)
);

create index idx_email
    on email_subscribers (email);

create index idx_status
    on email_subscribers (status);

create table permissions
(
    id          bigint auto_increment
        primary key,
    name        varchar(100)                        not null,
    slug        varchar(100)                        not null,
    description text                                null,
    module      varchar(50)                         not null,
    created_at  timestamp default CURRENT_TIMESTAMP null,
    updated_at  timestamp default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint name
        unique (name),
    constraint slug
        unique (slug)
);

create index idx_module
    on permissions (module);

create index idx_slug
    on permissions (slug);

create table roles
(
    id          bigint auto_increment
        primary key,
    name        varchar(50)                         not null,
    slug        varchar(50)                         not null,
    description text                                null,
    created_at  timestamp default CURRENT_TIMESTAMP null,
    updated_at  timestamp default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint name
        unique (name),
    constraint slug
        unique (slug)
);

create table role_permissions
(
    id            bigint auto_increment
        primary key,
    role_id       bigint                              not null,
    permission_id bigint                              not null,
    created_at    timestamp default CURRENT_TIMESTAMP null,
    constraint unique_role_permission
        unique (role_id, permission_id),
    constraint role_permissions_ibfk_1
        foreign key (role_id) references roles (id)
            on delete cascade,
    constraint role_permissions_ibfk_2
        foreign key (permission_id) references permissions (id)
            on delete cascade
);

create index permission_id
    on role_permissions (permission_id);

create index idx_slug
    on roles (slug);

create table seo_metadata
(
    id                  bigint auto_increment
        primary key,
    entity_type         enum ('post', 'category', 'tag', 'user') not null,
    entity_id           bigint                                   not null,
    meta_title          varchar(255)                             null,
    meta_description    text                                     null,
    meta_keywords       text                                     null,
    og_title            varchar(255)                             null,
    og_description      text                                     null,
    og_image_url        varchar(255)                             null,
    twitter_card        varchar(50)                              null,
    twitter_title       varchar(255)                             null,
    twitter_description text                                     null,
    twitter_image_url   varchar(255)                             null,
    created_at          timestamp default CURRENT_TIMESTAMP      null,
    updated_at          timestamp default CURRENT_TIMESTAMP      null on update CURRENT_TIMESTAMP,
    constraint unique_seo_entity
        unique (entity_type, entity_id)
);

create index idx_entity
    on seo_metadata (entity_type, entity_id);

create table settings
(
    id            bigint auto_increment
        primary key,
    setting_key   varchar(100)                          not null,
    setting_value text                                  null,
    setting_group varchar(50) default 'general'         null,
    is_public     tinyint(1)  default 1                 null,
    created_at    timestamp   default CURRENT_TIMESTAMP null,
    updated_at    timestamp   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint setting_key
        unique (setting_key)
);

create index idx_setting_group
    on settings (setting_group);

create index idx_setting_key
    on settings (setting_key);

create table tags
(
    id         bigint auto_increment
        primary key,
    name       varchar(50)                         not null,
    slug       varchar(60)                         not null,
    created_at timestamp default CURRENT_TIMESTAMP null,
    updated_at timestamp default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint slug
        unique (slug)
);

create index idx_slug
    on tags (slug);

create table users
(
    id            bigint auto_increment
        primary key,
    username      varchar(50)                          not null,
    email         varchar(100)                         not null,
    password_hash varchar(255)                         null,
    is_active     tinyint(1) default 1                 null,
    created_at    timestamp  default CURRENT_TIMESTAMP null,
    updated_at    timestamp  default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint email
        unique (email),
    constraint username
        unique (username)
);

create table api_tokens
(
    id           bigint auto_increment
        primary key,
    user_id      bigint                              not null,
    token        varchar(255)                        not null,
    name         varchar(100)                        not null,
    permissions  json                                null,
    last_used_at timestamp                           null,
    expires_at   timestamp                           null,
    created_at   timestamp default CURRENT_TIMESTAMP null,
    updated_at   timestamp default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint token
        unique (token),
    constraint api_tokens_ibfk_1
        foreign key (user_id) references users (id)
            on delete cascade
);

create index idx_token
    on api_tokens (token);

create index user_id
    on api_tokens (user_id);

create table followers
(
    id           bigint auto_increment
        primary key,
    follower_id  bigint                              not null,
    following_id bigint                              not null,
    created_at   timestamp default CURRENT_TIMESTAMP null,
    constraint unique_follow
        unique (follower_id, following_id),
    constraint followers_ibfk_1
        foreign key (follower_id) references users (id)
            on delete cascade,
    constraint followers_ibfk_2
        foreign key (following_id) references users (id)
            on delete cascade
);

create index following_id
    on followers (following_id);

create table likes
(
    id          bigint auto_increment
        primary key,
    user_id     bigint                              not null,
    entity_type enum ('post', 'comment')            not null,
    entity_id   bigint                              not null,
    created_at  timestamp default CURRENT_TIMESTAMP null,
    constraint unique_like
        unique (user_id, entity_type, entity_id),
    constraint likes_ibfk_1
        foreign key (user_id) references users (id)
            on delete cascade
);

create index idx_entity
    on likes (entity_type, entity_id);

create table media
(
    id          bigint auto_increment
        primary key,
    user_id     bigint                              not null,
    file_name   varchar(255)                        not null,
    file_path   varchar(255)                        not null,
    file_type   varchar(50)                         not null,
    mime_type   varchar(100)                        not null,
    file_size   bigint                              not null,
    alt_text    varchar(255)                        null,
    description text                                null,
    created_at  timestamp default CURRENT_TIMESTAMP null,
    updated_at  timestamp default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    public_id   varchar(255)                        null,
    width       int                                 null,
    height      int                                 null,
    constraint file_path
        unique (file_path),
    constraint media_ibfk_1
        foreign key (user_id) references users (id)
            on delete cascade
);

create index idx_file_type
    on media (file_type);

create index idx_user_id
    on media (user_id);

create table newsletters
(
    id           bigint auto_increment
        primary key,
    subject      varchar(255)                                                              not null,
    content      longtext                                                                  not null,
    sent_by      bigint                                                                    not null,
    status       enum ('draft', 'scheduled', 'sent', 'canceled') default 'draft'           null,
    sent_at      timestamp                                                                 null,
    scheduled_at timestamp                                                                 null,
    created_at   timestamp                                       default CURRENT_TIMESTAMP null,
    updated_at   timestamp                                       default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint newsletters_ibfk_1
        foreign key (sent_by) references users (id)
            on delete cascade
);

create table newsletter_stats
(
    id              bigint auto_increment
        primary key,
    newsletter_id   bigint                              not null,
    total_sent      int       default 0                 null,
    total_delivered int       default 0                 null,
    total_opened    int       default 0                 null,
    total_clicked   int       default 0                 null,
    created_at      timestamp default CURRENT_TIMESTAMP null,
    updated_at      timestamp default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint newsletter_stats_ibfk_1
        foreign key (newsletter_id) references newsletters (id)
            on delete cascade
);

create index newsletter_id
    on newsletter_stats (newsletter_id);

create index idx_scheduled_at
    on newsletters (scheduled_at);

create index idx_sent_at
    on newsletters (sent_at);

create index idx_status
    on newsletters (status);

create index sent_by
    on newsletters (sent_by);

create table notifications
(
    id          bigint auto_increment
        primary key,
    user_id     bigint                               not null,
    sender_id   bigint                               null,
    type        varchar(50)                          not null,
    entity_type varchar(50)                          not null,
    entity_id   bigint                               not null,
    message     text                                 not null,
    is_read     tinyint(1) default 0                 null,
    created_at  timestamp  default CURRENT_TIMESTAMP null,
    constraint notifications_ibfk_1
        foreign key (user_id) references users (id)
            on delete cascade,
    constraint notifications_ibfk_2
        foreign key (sender_id) references users (id)
            on delete set null
);

create index idx_is_read
    on notifications (is_read);

create index idx_user_id
    on notifications (user_id);

create index sender_id
    on notifications (sender_id);

create table posts
(
    id                 bigint auto_increment
        primary key,
    author_id          bigint                                                            not null,
    title              varchar(255)                                                      not null,
    slug               varchar(280)                                                      not null,
    excerpt            text                                                              null,
    content            longtext                                                          not null,
    featured_image_url varchar(255)                                                      null,
    status             enum ('draft', 'published', 'archived') default 'draft'           null,
    comment_status     enum ('open', 'closed')                 default 'open'            null,
    views_count        bigint                                  default 0                 null,
    created_at         timestamp                               default CURRENT_TIMESTAMP null,
    published_at       timestamp                                                         null,
    updated_at         timestamp                               default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    deleted_at         timestamp                                                         null,
    e_tag              text                                                              null comment 'e_tag',
    constraint slug
        unique (slug),
    constraint posts_ibfk_1
        foreign key (author_id) references users (id)
            on delete cascade
);

create table analytics
(
    id               bigint auto_increment
        primary key,
    post_id          bigint                                  not null,
    views            int           default 0                 null,
    unique_visitors  int           default 0                 null,
    avg_time_on_page int           default 0                 null,
    bounce_rate      decimal(5, 2) default 0.00              null,
    date             date                                    not null,
    created_at       timestamp     default CURRENT_TIMESTAMP null,
    updated_at       timestamp     default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint unique_post_date
        unique (post_id, date),
    constraint analytics_ibfk_1
        foreign key (post_id) references posts (id)
            on delete cascade
);

create index idx_date
    on analytics (date);

create table comments
(
    id         bigint auto_increment
        primary key,
    post_id    bigint                                                                  not null,
    user_id    bigint                                                                  not null,
    parent_id  bigint                                                                  null,
    content    text                                                                    not null,
    status     enum ('pending', 'approved', 'spam', 'trash') default 'pending'         null,
    created_at timestamp                                     default CURRENT_TIMESTAMP null,
    updated_at timestamp                                     default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint comments_ibfk_1
        foreign key (post_id) references posts (id)
            on delete cascade,
    constraint comments_ibfk_2
        foreign key (user_id) references users (id)
            on delete cascade,
    constraint comments_ibfk_3
        foreign key (parent_id) references comments (id)
            on delete cascade
);

create index idx_post_id
    on comments (post_id);

create index idx_status
    on comments (status);

create index idx_user_id
    on comments (user_id);

create index parent_id
    on comments (parent_id);

create table polls
(
    id             bigint auto_increment
        primary key,
    creator_id     bigint                                              not null,
    post_id        bigint                                              null,
    question       varchar(255)                                        not null,
    status         enum ('active', 'closed') default 'active'          null,
    allow_multiple tinyint(1)                default 0                 null,
    start_date     timestamp                 default CURRENT_TIMESTAMP null,
    end_date       timestamp                                           null,
    created_at     timestamp                 default CURRENT_TIMESTAMP null,
    updated_at     timestamp                 default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint polls_ibfk_1
        foreign key (creator_id) references users (id)
            on delete cascade,
    constraint polls_ibfk_2
        foreign key (post_id) references posts (id)
            on delete set null
);

create table poll_options
(
    id          bigint auto_increment
        primary key,
    poll_id     bigint                              not null,
    option_text varchar(255)                        not null,
    sort_order  int       default 0                 null,
    created_at  timestamp default CURRENT_TIMESTAMP null,
    constraint poll_options_ibfk_1
        foreign key (poll_id) references polls (id)
            on delete cascade
);

create index idx_poll_id
    on poll_options (poll_id);

create table poll_votes
(
    id         bigint auto_increment
        primary key,
    poll_id    bigint                              not null,
    option_id  bigint                              not null,
    user_id    bigint                              null,
    ip_address varchar(45)                         not null,
    created_at timestamp default CURRENT_TIMESTAMP null,
    constraint poll_votes_ibfk_1
        foreign key (poll_id) references polls (id)
            on delete cascade,
    constraint poll_votes_ibfk_2
        foreign key (option_id) references poll_options (id)
            on delete cascade,
    constraint poll_votes_ibfk_3
        foreign key (user_id) references users (id)
            on delete set null
);

create index idx_option_id
    on poll_votes (option_id);

create index idx_poll_id
    on poll_votes (poll_id);

create index user_id
    on poll_votes (user_id);

create index creator_id
    on polls (creator_id);

create index idx_post_id
    on polls (post_id);

create index idx_status
    on polls (status);

create table post_categories
(
    id          bigint auto_increment
        primary key,
    post_id     bigint                              not null,
    category_id bigint                              not null,
    created_at  timestamp default CURRENT_TIMESTAMP null,
    constraint unique_post_category
        unique (post_id, category_id),
    constraint post_categories_ibfk_1
        foreign key (post_id) references posts (id)
            on delete cascade,
    constraint post_categories_ibfk_2
        foreign key (category_id) references categories (id)
            on delete cascade
);

create index category_id
    on post_categories (category_id);

create table post_media
(
    id         bigint auto_increment
        primary key,
    post_id    bigint                              not null,
    media_id   bigint                              not null,
    sort_order int       default 0                 null,
    created_at timestamp default CURRENT_TIMESTAMP null,
    constraint unique_post_media
        unique (post_id, media_id),
    constraint post_media_ibfk_1
        foreign key (post_id) references posts (id)
            on delete cascade,
    constraint post_media_ibfk_2
        foreign key (media_id) references media (id)
            on delete cascade
);

create index media_id
    on post_media (media_id);

create table post_tags
(
    id         bigint auto_increment
        primary key,
    post_id    bigint                              not null,
    tag_id     bigint                              not null,
    created_at timestamp default CURRENT_TIMESTAMP null,
    constraint unique_post_tag
        unique (post_id, tag_id),
    constraint post_tags_ibfk_1
        foreign key (post_id) references posts (id)
            on delete cascade,
    constraint post_tags_ibfk_2
        foreign key (tag_id) references tags (id)
            on delete cascade
);

create index tag_id
    on post_tags (tag_id);

create index idx_author
    on posts (author_id);

create index idx_created_at
    on posts (created_at);

create index idx_published_at
    on posts (published_at);

create index idx_slug
    on posts (slug);

create index idx_status
    on posts (status);

create table revisions
(
    id              bigint auto_increment
        primary key,
    post_id         bigint                              not null,
    user_id         bigint                              not null,
    content         longtext                            not null,
    revision_number int                                 not null,
    created_at      timestamp default CURRENT_TIMESTAMP null,
    constraint unique_post_revision
        unique (post_id, revision_number),
    constraint revisions_ibfk_1
        foreign key (post_id) references posts (id)
            on delete cascade,
    constraint revisions_ibfk_2
        foreign key (user_id) references users (id)
            on delete cascade
);

create index idx_post_id
    on revisions (post_id);

create index user_id
    on revisions (user_id);

create table saved_posts
(
    id         bigint auto_increment
        primary key,
    user_id    bigint                              not null,
    post_id    bigint                              not null,
    created_at timestamp default CURRENT_TIMESTAMP null,
    constraint unique_saved_post
        unique (user_id, post_id),
    constraint saved_posts_ibfk_1
        foreign key (user_id) references users (id)
            on delete cascade,
    constraint saved_posts_ibfk_2
        foreign key (post_id) references posts (id)
            on delete cascade
);

create index post_id
    on saved_posts (post_id);

create table series
(
    id          bigint auto_increment
        primary key,
    author_id   bigint                                                            not null,
    title       varchar(255)                                                      not null,
    slug        varchar(280)                                                      not null,
    description text                                                              null,
    status      enum ('draft', 'published', 'archived') default 'published'       null,
    created_at  timestamp                               default CURRENT_TIMESTAMP null,
    updated_at  timestamp                               default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint slug
        unique (slug),
    constraint series_ibfk_1
        foreign key (author_id) references users (id)
            on delete cascade
);

create index idx_author
    on series (author_id);

create index idx_slug
    on series (slug);

create table series_posts
(
    id         bigint auto_increment
        primary key,
    series_id  bigint                              not null,
    post_id    bigint                              not null,
    position   int                                 not null,
    created_at timestamp default CURRENT_TIMESTAMP null,
    constraint unique_series_post
        unique (series_id, post_id),
    constraint unique_series_post_position
        unique (series_id, position),
    constraint series_posts_ibfk_1
        foreign key (series_id) references series (id)
            on delete cascade,
    constraint series_posts_ibfk_2
        foreign key (post_id) references posts (id)
            on delete cascade
);

create index post_id
    on series_posts (post_id);

create table social_auth
(
    id               bigint auto_increment
        primary key,
    user_id          bigint                              not null,
    provider         varchar(20)                         not null,
    provider_user_id varchar(100)                        not null,
    access_token     text                                null,
    refresh_token    text                                null,
    created_at       timestamp default CURRENT_TIMESTAMP null,
    updated_at       timestamp default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint unique_social_account
        unique (provider, provider_user_id),
    constraint social_auth_ibfk_1
        foreign key (user_id) references users (id)
            on delete cascade
);

create index user_id
    on social_auth (user_id);

create table user_permissions
(
    id            bigint auto_increment
        primary key,
    user_id       bigint                               not null,
    permission_id bigint                               not null,
    is_granted    tinyint(1) default 1                 null,
    created_at    timestamp  default CURRENT_TIMESTAMP null,
    updated_at    timestamp  default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint unique_user_permission
        unique (user_id, permission_id),
    constraint user_permissions_ibfk_1
        foreign key (user_id) references users (id)
            on delete cascade,
    constraint user_permissions_ibfk_2
        foreign key (permission_id) references permissions (id)
            on delete cascade
);

create index permission_id
    on user_permissions (permission_id);

create table user_profiles
(
    id           bigint auto_increment
        primary key,
    user_id      bigint                              not null,
    display_name varchar(100)                        null,
    bio          text                                null,
    avatar_url   varchar(255)                        null,
    website      varchar(255)                        null,
    social_links json                                null,
    location     varchar(100)                        null,
    created_at   timestamp default CURRENT_TIMESTAMP null,
    updated_at   timestamp default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    constraint user_id
        unique (user_id),
    constraint user_profiles_ibfk_1
        foreign key (user_id) references users (id)
            on delete cascade
);

create table user_roles
(
    id         bigint auto_increment
        primary key,
    user_id    bigint                              not null,
    role_id    bigint                              not null,
    created_at timestamp default CURRENT_TIMESTAMP null,
    constraint unique_user_role
        unique (user_id, role_id),
    constraint user_roles_ibfk_1
        foreign key (user_id) references users (id)
            on delete cascade,
    constraint user_roles_ibfk_2
        foreign key (role_id) references roles (id)
            on delete cascade
);

create index role_id
    on user_roles (role_id);

create table user_sessions
(
    id            bigint auto_increment
        primary key,
    user_id       bigint                              not null,
    session_token varchar(255)                        not null,
    ip_address    varchar(45)                         null,
    user_agent    text                                null,
    expires_at    timestamp                           not null,
    created_at    timestamp default CURRENT_TIMESTAMP null,
    constraint session_token
        unique (session_token),
    constraint user_sessions_ibfk_1
        foreign key (user_id) references users (id)
            on delete cascade
);

create index idx_expires_at
    on user_sessions (expires_at);

create index idx_session_token
    on user_sessions (session_token);

create index user_id
    on user_sessions (user_id);


