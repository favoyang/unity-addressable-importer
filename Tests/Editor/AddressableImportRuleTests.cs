
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityAddressableImporter.Helper;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UnityAddressableImporter.Tests
{
    public class AddressableImportRuleTests
    {

        [Test]
        public void MatchWildcardTest()
        {
            AddressableImportRule rule = new AddressableImportRule();
            rule.matchType = AddressableImportRuleMatchType.Wildcard;
            // Raw path
            rule.path = "Assets/Sprites/";
            Assert.IsTrue(rule.Match("Assets/Sprites/cat/cat.png"));
            Assert.IsFalse(rule.Match("Assets/Fbx/cat/cat.fbx"));
            // '*' wildcard
            rule.path = "Assets/Sprites/*/*.png";
            Assert.IsTrue(rule.Match("Assets/Sprites/cat/cat.png"));
            Assert.IsFalse(rule.Match("Assets/Sprites/cat/cat.jpg"));
            // '?' wildcard
            rule.path = "Assets/Sprites/*/???.png";
            Assert.IsTrue(rule.Match("Assets/Sprites/cat/cat.png"));
            Assert.IsFalse(rule.Match("Assets/Sprites/cat/bird.png"));
            // rule.groupName = "";
            // rule.LabelMode = LabelWriteMode.Add;
            // rule.simplified = true;
            // rule.addressReplacement = "";
        }

        [Test]
        public void MatchRegexTest()
        {
            AddressableImportRule rule = new AddressableImportRule();
            rule.matchType = AddressableImportRuleMatchType.Regex;
            rule.path = @"Assets/Sprites/.*/.*\.png";
            Assert.IsTrue(rule.Match("Assets/Sprites/cat/cat.png"));
            Assert.IsFalse(rule.Match("Assets/Sprites/cat/cat.jpg"));
        }

        [Test]
        public void ParseGroupReplacementTest()
        {
            AddressableImportRule rule = new AddressableImportRule();
            // Test empty path
            rule.matchType = AddressableImportRuleMatchType.Wildcard;
            rule.path = "";
            rule.groupName = "somegroup";
            Assert.IsNull(rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            rule.path = " ";
            rule.groupName = "somegroup";
            Assert.IsNull(rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            // Test empty groupName
            rule.matchType = AddressableImportRuleMatchType.Wildcard;
            rule.path = "Assets/Sprites/*/*.png";
            rule.groupName = "";
            Assert.IsNull(rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            rule.path = "Assets/Sprites/*/*.png";
            rule.groupName = " ";
            Assert.IsNull(rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            // Test empty spaces in groupName
            rule.matchType = AddressableImportRuleMatchType.Wildcard;
            rule.path = "Assets/Sprites/*/*.png";
            rule.groupName = " group-a";
            Assert.AreEqual("group-a", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            rule.groupName = "group-a ";
            Assert.AreEqual("group-a", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            rule.groupName = " group-a ";
            Assert.AreEqual("group-a", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            // Test splash in groupName
            rule.matchType = AddressableImportRuleMatchType.Wildcard;
            rule.path = "Assets/Sprites/*/*.png";
            rule.groupName = "group/a";
            Assert.AreEqual("group-a", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            rule.groupName = @"group\a ";
            Assert.AreEqual("group-a", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            // Test static groupName
            rule.matchType = AddressableImportRuleMatchType.Wildcard;
            rule.path = "Assets/Sprites/*/*.png";
            rule.groupName = "group-a";
            Assert.AreEqual("group-a", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            // Test wildcard + path elements
            rule.matchType = AddressableImportRuleMatchType.Wildcard;
            rule.path = "Assets/Sprites/*/*.png";
            rule.groupName = "${PATH[0]}";
            Assert.AreEqual("Assets", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            rule.groupName = "${PATH[1]}";
            Assert.AreEqual("Sprites", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            rule.groupName = "${PATH[-1]}";
            Assert.AreEqual("cat", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            rule.groupName = "${PATH[0]}:${PATH[-1]}-group";
            Assert.AreEqual("Assets:cat-group", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            // Test regex + path elements
            rule.matchType = AddressableImportRuleMatchType.Regex;
            rule.path = @"Assets/Sprites/(.*)/(.*)\.png";
            rule.groupName = "${PATH[0]}";
            Assert.AreEqual("Assets", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            rule.groupName = "${PATH[1]}";
            Assert.AreEqual("Sprites", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            rule.groupName = "${PATH[-1]}";
            Assert.AreEqual("cat", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            rule.groupName = "${PATH[0]}:${PATH[-1]}-group";
            Assert.AreEqual("Assets:cat-group", rule.ParseGroupReplacement("Assets/Sprites/cat/cat.png"));
            // Test regex + unnamed groups
            rule.matchType = AddressableImportRuleMatchType.Regex;
            rule.path = @"Assets/Sprites/(.*)/(.*)\.png";
            rule.groupName = "${PATH[0]}:$1-$2";
            Assert.AreEqual("Assets:cat-foo", rule.ParseGroupReplacement("Assets/Sprites/cat/foo.png"));
            // Test regex + named groups
            rule.matchType = AddressableImportRuleMatchType.Regex;
            rule.path = @"Assets/Sprites/(?<category>.*)/(?<asset>.*)\.png";
            rule.groupName = "${PATH[0]}:${category}-${asset}";
            Assert.AreEqual("Assets:cat-foo", rule.ParseGroupReplacement("Assets/Sprites/cat/foo.png"));
        }

        [Test]
        public void AddressSimplifyTest()
        {
            AddressableImportRule rule = new AddressableImportRule();
            // Test wildcard + simplify
            rule.matchType = AddressableImportRuleMatchType.Wildcard;
            rule.path = "Assets/Sprites/*/*.png";
            rule.simplified = true;
            Assert.AreEqual("cat", rule.ParseAddressReplacement("Assets/Sprites/cat/cat.png"));
            // Test regex + simplify
            rule.matchType = AddressableImportRuleMatchType.Regex;
            rule.path = @"Assets/Sprites/(.*)/(.*)\.png";
            rule.simplified = true;
            Assert.AreEqual("cat", rule.ParseAddressReplacement("Assets/Sprites/cat/cat.png"));
            // Test simplify + non empty address replacement
            rule.matchType = AddressableImportRuleMatchType.Regex;
            rule.addressReplacement = "somevalue";
            rule.path = @"Assets/Sprites/(.*)/(.*)\.png";
            rule.simplified = true;
            Assert.AreEqual("cat", rule.ParseAddressReplacement("Assets/Sprites/cat/cat.png"));
        }

        [Test]
        public void ParseAddressReplacementTest()
        {
            AddressableImportRule rule = new AddressableImportRule();
            // Test empty path
            rule.matchType = AddressableImportRuleMatchType.Regex;
            rule.path = "";
            Assert.AreEqual("Assets/Sprites/cat/cat.png", rule.ParseAddressReplacement("Assets/Sprites/cat/cat.png"));
            rule.path = " ";
            Assert.AreEqual("Assets/Sprites/cat/cat.png", rule.ParseAddressReplacement("Assets/Sprites/cat/cat.png"));
            // Test empty address replacement
            rule.matchType = AddressableImportRuleMatchType.Regex;
            rule.path = @"Assets/Sprites/(.*)/(.*)\.png";
            rule.addressReplacement = "";
            Assert.AreEqual("Assets/Sprites/cat/cat.png", rule.ParseAddressReplacement("Assets/Sprites/cat/cat.png"));
            rule.addressReplacement = " ";
            Assert.AreEqual("Assets/Sprites/cat/cat.png", rule.ParseAddressReplacement("Assets/Sprites/cat/cat.png"));
            // Test regex + path elements
            rule.matchType = AddressableImportRuleMatchType.Regex;
            rule.path = @"Assets/Sprites/(.*)/(.*)\.png";
            rule.addressReplacement = "${PATH[0]}";
            Assert.AreEqual("Assets", rule.ParseAddressReplacement("Assets/Sprites/cat/cat.png"));
            rule.addressReplacement = "${PATH[1]}";
            Assert.AreEqual("Sprites", rule.ParseAddressReplacement("Assets/Sprites/cat/cat.png"));
            rule.addressReplacement = "${PATH[-1]}";
            Assert.AreEqual("cat", rule.ParseAddressReplacement("Assets/Sprites/cat/cat.png"));
            rule.addressReplacement = "${PATH[0]}:${PATH[-1]}-element";
            Assert.AreEqual("Assets:cat-element", rule.ParseAddressReplacement("Assets/Sprites/cat/cat.png"));
            // Test regex + unnamed groups
            rule.matchType = AddressableImportRuleMatchType.Regex;
            rule.path = @"Assets/Sprites/(.*)/(.*)\.png";
            rule.addressReplacement = "${PATH[0]}:$1-$2";
            Assert.AreEqual("Assets:cat-foo", rule.ParseAddressReplacement("Assets/Sprites/cat/foo.png"));
            // Test regex + named groups
            rule.matchType = AddressableImportRuleMatchType.Regex;
            rule.path = @"Assets/Sprites/(?<category>.*)/(?<asset>.*)\.png";
            rule.addressReplacement = "${PATH[0]}:${category}-${asset}";
            Assert.AreEqual("Assets:cat-foo", rule.ParseAddressReplacement("Assets/Sprites/cat/foo.png"));
        }
    }
}