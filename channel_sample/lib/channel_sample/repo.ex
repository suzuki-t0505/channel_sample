defmodule ChannelSample.Repo do
  use Ecto.Repo,
    otp_app: :channel_sample,
    adapter: Ecto.Adapters.Postgres
end
