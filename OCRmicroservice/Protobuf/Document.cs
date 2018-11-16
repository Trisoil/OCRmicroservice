// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: document.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from document.proto</summary>
public static partial class DocumentReflection {

  #region Descriptor
  /// <summary>File descriptor for document.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static DocumentReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "Cg5kb2N1bWVudC5wcm90bxoNY291bnRyeS5wcm90byJkCgxJQ0FPRG9jdW1l",
          "bnQSJAoNZG9jdW1lbnRfdHlwZRgBIAEoDjINLkRvY3VtZW50VHlwZRIZCgdj",
          "b3VudHJ5GAIgASgOMgguQ291bnRyeRITCgtkZXNjcmlwdGlvbhgDIAEoCSJq",
          "ChJHb3Zlcm5tZW50SXNzdWVkSWQSJAoNZG9jdW1lbnRfdHlwZRgBIAEoDjIN",
          "LkRvY3VtZW50VHlwZRIZCgdjb3VudHJ5GAIgASgOMgguQ291bnRyeRITCgtk",
          "ZXNjcmlwdGlvbhgDIAEoCSJDChFTZWNvbmRhcnlEb2N1bWVudBIZCgdjb3Vu",
          "dHJ5GAEgASgOMgguQ291bnRyeRITCgtkZXNjcmlwdGlvbhgCIAEoCSpqCgxE",
          "b2N1bWVudFR5cGUSFwoTVW5rbm93bkRvY3VtZW50VHlwZRAAEhAKDElkZW50",
          "aXR5Q2FyZBABEgwKCFBhc3Nwb3J0EAISEgoORHJpdmluZ0xpY2VuY2UQAxIN",
          "CglTZWNvbmRhcnkQBGIGcHJvdG8z"));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { global::CountryReflection.Descriptor, },
        new pbr::GeneratedClrTypeInfo(new[] {typeof(global::DocumentType), }, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::ICAODocument), global::ICAODocument.Parser, new[]{ "DocumentType", "Country", "Description" }, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::GovernmentIssuedId), global::GovernmentIssuedId.Parser, new[]{ "DocumentType", "Country", "Description" }, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::SecondaryDocument), global::SecondaryDocument.Parser, new[]{ "Country", "Description" }, null, null, null)
        }));
  }
  #endregion

}
#region Enums
public enum DocumentType {
  [pbr::OriginalName("UnknownDocumentType")] UnknownDocumentType = 0,
  [pbr::OriginalName("IdentityCard")] IdentityCard = 1,
  [pbr::OriginalName("Passport")] Passport = 2,
  [pbr::OriginalName("DrivingLicence")] DrivingLicence = 3,
  [pbr::OriginalName("Secondary")] Secondary = 4,
}

#endregion

#region Messages
public sealed partial class ICAODocument : pb::IMessage<ICAODocument> {
  private static readonly pb::MessageParser<ICAODocument> _parser = new pb::MessageParser<ICAODocument>(() => new ICAODocument());
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<ICAODocument> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::DocumentReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ICAODocument() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ICAODocument(ICAODocument other) : this() {
    documentType_ = other.documentType_;
    country_ = other.country_;
    description_ = other.description_;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ICAODocument Clone() {
    return new ICAODocument(this);
  }

  /// <summary>Field number for the "document_type" field.</summary>
  public const int DocumentTypeFieldNumber = 1;
  private global::DocumentType documentType_ = 0;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::DocumentType DocumentType {
    get { return documentType_; }
    set {
      documentType_ = value;
    }
  }

  /// <summary>Field number for the "country" field.</summary>
  public const int CountryFieldNumber = 2;
  private global::Country country_ = 0;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::Country Country {
    get { return country_; }
    set {
      country_ = value;
    }
  }

  /// <summary>Field number for the "description" field.</summary>
  public const int DescriptionFieldNumber = 3;
  private string description_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Description {
    get { return description_; }
    set {
      description_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as ICAODocument);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(ICAODocument other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (DocumentType != other.DocumentType) return false;
    if (Country != other.Country) return false;
    if (Description != other.Description) return false;
    return true;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (DocumentType != 0) hash ^= DocumentType.GetHashCode();
    if (Country != 0) hash ^= Country.GetHashCode();
    if (Description.Length != 0) hash ^= Description.GetHashCode();
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (DocumentType != 0) {
      output.WriteRawTag(8);
      output.WriteEnum((int) DocumentType);
    }
    if (Country != 0) {
      output.WriteRawTag(16);
      output.WriteEnum((int) Country);
    }
    if (Description.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(Description);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (DocumentType != 0) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) DocumentType);
    }
    if (Country != 0) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Country);
    }
    if (Description.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Description);
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(ICAODocument other) {
    if (other == null) {
      return;
    }
    if (other.DocumentType != 0) {
      DocumentType = other.DocumentType;
    }
    if (other.Country != 0) {
      Country = other.Country;
    }
    if (other.Description.Length != 0) {
      Description = other.Description;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          input.SkipLastField();
          break;
        case 8: {
          documentType_ = (global::DocumentType) input.ReadEnum();
          break;
        }
        case 16: {
          country_ = (global::Country) input.ReadEnum();
          break;
        }
        case 26: {
          Description = input.ReadString();
          break;
        }
      }
    }
  }

}

public sealed partial class GovernmentIssuedId : pb::IMessage<GovernmentIssuedId> {
  private static readonly pb::MessageParser<GovernmentIssuedId> _parser = new pb::MessageParser<GovernmentIssuedId>(() => new GovernmentIssuedId());
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<GovernmentIssuedId> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::DocumentReflection.Descriptor.MessageTypes[1]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public GovernmentIssuedId() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public GovernmentIssuedId(GovernmentIssuedId other) : this() {
    documentType_ = other.documentType_;
    country_ = other.country_;
    description_ = other.description_;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public GovernmentIssuedId Clone() {
    return new GovernmentIssuedId(this);
  }

  /// <summary>Field number for the "document_type" field.</summary>
  public const int DocumentTypeFieldNumber = 1;
  private global::DocumentType documentType_ = 0;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::DocumentType DocumentType {
    get { return documentType_; }
    set {
      documentType_ = value;
    }
  }

  /// <summary>Field number for the "country" field.</summary>
  public const int CountryFieldNumber = 2;
  private global::Country country_ = 0;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::Country Country {
    get { return country_; }
    set {
      country_ = value;
    }
  }

  /// <summary>Field number for the "description" field.</summary>
  public const int DescriptionFieldNumber = 3;
  private string description_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Description {
    get { return description_; }
    set {
      description_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as GovernmentIssuedId);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(GovernmentIssuedId other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (DocumentType != other.DocumentType) return false;
    if (Country != other.Country) return false;
    if (Description != other.Description) return false;
    return true;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (DocumentType != 0) hash ^= DocumentType.GetHashCode();
    if (Country != 0) hash ^= Country.GetHashCode();
    if (Description.Length != 0) hash ^= Description.GetHashCode();
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (DocumentType != 0) {
      output.WriteRawTag(8);
      output.WriteEnum((int) DocumentType);
    }
    if (Country != 0) {
      output.WriteRawTag(16);
      output.WriteEnum((int) Country);
    }
    if (Description.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(Description);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (DocumentType != 0) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) DocumentType);
    }
    if (Country != 0) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Country);
    }
    if (Description.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Description);
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(GovernmentIssuedId other) {
    if (other == null) {
      return;
    }
    if (other.DocumentType != 0) {
      DocumentType = other.DocumentType;
    }
    if (other.Country != 0) {
      Country = other.Country;
    }
    if (other.Description.Length != 0) {
      Description = other.Description;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          input.SkipLastField();
          break;
        case 8: {
          documentType_ = (global::DocumentType) input.ReadEnum();
          break;
        }
        case 16: {
          country_ = (global::Country) input.ReadEnum();
          break;
        }
        case 26: {
          Description = input.ReadString();
          break;
        }
      }
    }
  }

}

public sealed partial class SecondaryDocument : pb::IMessage<SecondaryDocument> {
  private static readonly pb::MessageParser<SecondaryDocument> _parser = new pb::MessageParser<SecondaryDocument>(() => new SecondaryDocument());
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<SecondaryDocument> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::DocumentReflection.Descriptor.MessageTypes[2]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public SecondaryDocument() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public SecondaryDocument(SecondaryDocument other) : this() {
    country_ = other.country_;
    description_ = other.description_;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public SecondaryDocument Clone() {
    return new SecondaryDocument(this);
  }

  /// <summary>Field number for the "country" field.</summary>
  public const int CountryFieldNumber = 1;
  private global::Country country_ = 0;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::Country Country {
    get { return country_; }
    set {
      country_ = value;
    }
  }

  /// <summary>Field number for the "description" field.</summary>
  public const int DescriptionFieldNumber = 2;
  private string description_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Description {
    get { return description_; }
    set {
      description_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as SecondaryDocument);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(SecondaryDocument other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Country != other.Country) return false;
    if (Description != other.Description) return false;
    return true;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Country != 0) hash ^= Country.GetHashCode();
    if (Description.Length != 0) hash ^= Description.GetHashCode();
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (Country != 0) {
      output.WriteRawTag(8);
      output.WriteEnum((int) Country);
    }
    if (Description.Length != 0) {
      output.WriteRawTag(18);
      output.WriteString(Description);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Country != 0) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Country);
    }
    if (Description.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Description);
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(SecondaryDocument other) {
    if (other == null) {
      return;
    }
    if (other.Country != 0) {
      Country = other.Country;
    }
    if (other.Description.Length != 0) {
      Description = other.Description;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          input.SkipLastField();
          break;
        case 8: {
          country_ = (global::Country) input.ReadEnum();
          break;
        }
        case 18: {
          Description = input.ReadString();
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code
