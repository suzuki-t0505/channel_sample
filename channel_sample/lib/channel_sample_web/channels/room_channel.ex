defmodule ChannelSampleWeb.RoomChannel do
  require Logger
  use ChannelSampleWeb, :channel

  def join("room:lobby", message, socket) do
    Logger.debug(message)
    {:ok, socket}
  end

  def join("room:" <> _private_room_id, _message, _socket) do
    {:error, %{reason: "unauthorized"}}
  end

  def handle_in("new_msg", %{"body" => body}, socket) do
    Logger.debug(body)
    broadcast(socket, "new_msg", %{body: body})

    {:noreply, socket}
  end
end
