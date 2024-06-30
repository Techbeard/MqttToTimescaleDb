namespace MqttToTimescale
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Globalization;

    public partial class JanitzaJson
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("SN")]
        public string Sn { get; set; }

        [JsonPropertyName("CT_prim")]
        public string CtPrim { get; set; }

        [JsonPropertyName("CT_sec")]
        public string CtSec { get; set; }

        [JsonPropertyName("I_N")]
        public long IN { get; set; }

        [JsonPropertyName("Freq")]
        public long Freq { get; set; }

        [JsonPropertyName("Rotation")]
        public long Rotation { get; set; }

        [JsonPropertyName("U_LN")]
        public Phases ULn { get; set; }

        [JsonPropertyName("U_LL")]
        public PhasePhase ULl { get; set; }

        [JsonPropertyName("I")]
        public Phases I { get; set; }

        [JsonPropertyName("P")]
        public Phases P { get; set; }

        [JsonPropertyName("Q")]
        public PhaseAll Q { get; set; }

        [JsonPropertyName("S")]
        public PhaseAll S { get; set; }

        [JsonPropertyName("PhaseAll")]
        public Phases CosPhi { get; set; }

        [JsonPropertyName("Wp")]
        public PhaseAll Wp { get; set; }

        [JsonPropertyName("Wp_Consumed")]
        public PhaseAll WpConsumed { get; set; }

        [JsonPropertyName("Wp_Delivered")]
        public PhaseAll WpDelivered { get; set; }

        [JsonPropertyName("Wq")]
        public PhaseAll Wq { get; set; }

        [JsonPropertyName("Ws")]
        public PhaseAll Ws { get; set; }

        [JsonPropertyName("Ws_ind")]
        public PhaseAll WsInd { get; set; }

        [JsonPropertyName("Ws_cap")]
        public PhaseAll WsCap { get; set; }

        [JsonPropertyName("THD_I")]
        public Phases ThdI { get; set; }

        [JsonPropertyName("THD_U")]
        public Phases ThdU { get; set; }
    }

    public partial class PhaseAll
    {
        [JsonPropertyName("L1")]
        public long L1 { get; set; }

        [JsonPropertyName("L2")]
        public long L2 { get; set; }

        [JsonPropertyName("L3")]
        public long L3 { get; set; }

        [JsonPropertyName("All")]
        public long? All { get; set; }
    }

    public partial class Phases
    {
        [JsonPropertyName("L1")]
        public long L1 { get; set; }

         [JsonPropertyName("L2")]
        public long L2 { get; set; }

         [JsonPropertyName("L3")]
        public long L3 { get; set; }

    
    }


       public partial class PhasePhase
    {
        [JsonPropertyName("L1L2")]
        public long L1L2 { get; set; }

         [JsonPropertyName("L2L3")]
        public long L2L3 { get; set; }

         [JsonPropertyName("L3L1")]
        public long L3L1 { get; set; }

    
    }
}
