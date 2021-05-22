using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AntActor.Chat.Backend.Ants.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AntActor.Chat.Backend.DAL
{
    public class MessageRepository
    {
        private SqlConnection _connection;
        public MessageRepository(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public async Task<IEnumerable<Message>> GetAllForChannelAsync(string channelName)
        {
            var sql = "SELECT * FROM Message " 
                    + "WHERE Channel = @ChannelName "
                    + "ORDER BY Created";
            return await _connection.QueryAsync<Message>(sql, new { ChannelName = channelName });
        }

        public async Task AddAsync(Message message)
        {
            var sql = "INSERT INTO Message " 
                    + "VALUES(@Id, @Created, @Author, @Text, @Channel)";

            await _connection.ExecuteAsync(
                sql,
                new
                {
                    Id = Guid.NewGuid(),
                    message.Created,
                    message.Author,
                    message.Text,
                    message.Channel
                });
        }
    }
}