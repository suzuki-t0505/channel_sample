defmodule ChannelSample.Application do
  # See https://hexdocs.pm/elixir/Application.html
  # for more information on OTP Applications
  @moduledoc false

  use Application

  @impl true
  def start(_type, _args) do
    children = [
      ChannelSampleWeb.Telemetry,
      ChannelSample.Repo,
      {DNSCluster, query: Application.get_env(:channel_sample, :dns_cluster_query) || :ignore},
      {Phoenix.PubSub, name: ChannelSample.PubSub},
      # Start the Finch HTTP client for sending emails
      {Finch, name: ChannelSample.Finch},
      # Start a worker by calling: ChannelSample.Worker.start_link(arg)
      # {ChannelSample.Worker, arg},
      # Start to serve requests, typically the last entry
      ChannelSampleWeb.Endpoint
    ]

    # See https://hexdocs.pm/elixir/Supervisor.html
    # for other strategies and supported options
    opts = [strategy: :one_for_one, name: ChannelSample.Supervisor]
    Supervisor.start_link(children, opts)
  end

  # Tell Phoenix to update the endpoint configuration
  # whenever the application is updated.
  @impl true
  def config_change(changed, _new, removed) do
    ChannelSampleWeb.Endpoint.config_change(changed, removed)
    :ok
  end
end
