defmodule ChannelSampleWeb.RoomChannel do
  use ChannelSampleWeb, :channel

  def join("room:lobby", _message, socket) do
    {:ok, socket}
  end

  def join("room:" <> _private_room_id, _message, _socket) do
    {:error, %{reason: "unauthorized"}}
  end

  def handle_in("new_msg", %{"body" => body}, socket) do
    broadcast(socket, "new_msg", %{body: body})

    {:noreply, socket}
  end
end