using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Responses.Negotiation;
    using ProtoBuf;

    public class ProtoBufProcessor : IResponseProcessor
    {
        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            // MediaRange.FromString(string)は古い形式です： Please use the constructor
            if (requestedMediaRange.Matches(new MediaRange("application/x-protobuf")))
            //if (requestedMediaRange.Matches(MediaRange.FromString("application/x-protobuf")))
                return new ProcessorMatch { ModelResult = MatchResult.DontCare, RequestedContentTypeResult = MatchResult.ExactMatch };

            if (requestedMediaRange.Subtype.ToString().EndsWith("protobuf"))
                return new ProcessorMatch { ModelResult = MatchResult.DontCare, RequestedContentTypeResult = MatchResult.NonExactMatch };

            return new ProcessorMatch { ModelResult = MatchResult.DontCare, RequestedContentTypeResult = MatchResult.NoMatch };
        }

        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return new Response
            {
                Contents = stream => Serializer.Serialize(stream, model),
                ContentType = "application/x-protobuf"
            };
        }

        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get { return new[] { new Tuple<string, MediaRange>(".protobuf", new MediaRange("application/x-protobuf")) }; }
            //get { return new[] { new Tuple<string, MediaRange>(".protobuf", MediaRange.FromString("application/x-protobuf")) }; }
        }
    }

    public class ProtobufBodyDeserializer : IBodyDeserializer
    {
        // Interfaceの要件を満たすため、引数に`BindingContext`を追加
        public bool CanDeserialize(string contentType, BindingContext content)
        {
            return contentType == "application/x-protobuf";
        }

        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            return Serializer.NonGeneric.Deserialize(context.DestinationType, bodyStream);
        }
    }
}